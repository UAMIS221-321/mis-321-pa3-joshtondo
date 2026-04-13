using CryptoBot.Data;
using CryptoBot.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoBot.Services;

public interface IRagService
{
    Task<string> GetRelevantContextAsync(string query);
}

public class RagService(AppDbContext db, ILogger<RagService> logger) : IRagService
{
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "a", "an", "the", "is", "it", "in", "on", "at", "to", "for", "of", "and", "or",
        "what", "how", "why", "when", "where", "who", "me", "tell", "about", "does", "do",
        "can", "will", "are", "was", "be", "give", "get", "show", "explain", "i", "my",
        "please", "help", "with", "this", "that", "some", "more", "much"
    };

    public async Task<string> GetRelevantContextAsync(string query)
    {
        try
        {
            var allDocs = await db.KnowledgeDocuments.ToListAsync();
            if (allDocs.Count == 0)
            {
                logger.LogWarning("RAG: No knowledge documents found in database.");
                return string.Empty;
            }

            var queryWords = query.ToLower()
                .Split(new[] { ' ', ',', '?', '!', '.', '\'', '"' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length >= 2 && !StopWords.Contains(w))
                .ToList();

            if (queryWords.Count == 0)
                return string.Empty;

            var scored = allDocs.Select(doc =>
            {
                var titleLower    = doc.Title.ToLower();
                var keywordsLower = doc.Keywords.ToLower();
                var contentLower  = doc.Content.ToLower();

                var score = queryWords.Sum(word =>
                {
                    int s = 0;
                    if (titleLower.Contains(word))    s += 5; // title match = high value
                    if (keywordsLower.Contains(word)) s += 3; // keyword match
                    if (contentLower.Contains(word))  s += 1; // content match
                    return s;
                });

                return new { Doc = doc, Score = score };
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Take(3)
            .ToList();

            if (scored.Count == 0)
            {
                logger.LogInformation("RAG: No relevant documents for query: {Query}", query);
                return string.Empty;
            }

            logger.LogInformation("RAG: Retrieved {Count} docs for query '{Query}': {Titles}",
                scored.Count, query, string.Join(", ", scored.Select(x => x.Doc.Title)));

            var context = string.Join("\n\n---\n\n", scored.Select((x, i) =>
                $"[SOURCE {i + 1}: {x.Doc.Title}]\n{x.Doc.Content}"));

            return context;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving RAG context");
            return string.Empty;
        }
    }
}
