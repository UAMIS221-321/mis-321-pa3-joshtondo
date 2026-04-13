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
    public async Task<string> GetRelevantContextAsync(string query)
    {
        try
        {
            List<KnowledgeDocument> docs;

            // Try MySQL FULLTEXT search first
            try
            {
                var sanitized = SanitizeQuery(query);
                docs = await db.KnowledgeDocuments
                    .FromSqlRaw(
                        "SELECT * FROM KnowledgeDocuments WHERE MATCH(Title, Content, Keywords) AGAINST({0} IN BOOLEAN MODE) LIMIT 3",
                        sanitized)
                    .ToListAsync();
            }
            catch
            {
                // Fallback: keyword matching on Keywords column
                docs = [];
            }

            // Fallback: simple keyword matching if FULLTEXT returns nothing
            if (docs.Count == 0)
            {
                var queryWords = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var allDocs = await db.KnowledgeDocuments.ToListAsync();
                docs = allDocs
                    .Select(d => new
                    {
                        Doc = d,
                        Score = queryWords.Count(w =>
                            d.Keywords.Contains(w, StringComparison.OrdinalIgnoreCase) ||
                            d.Title.Contains(w, StringComparison.OrdinalIgnoreCase))
                    })
                    .Where(x => x.Score > 0)
                    .OrderByDescending(x => x.Score)
                    .Take(3)
                    .Select(x => x.Doc)
                    .ToList();
            }

            if (docs.Count == 0)
                return string.Empty;

            var context = string.Join("\n\n---\n\n", docs.Select(d =>
                $"## {d.Title}\n{d.Content}"));

            return $"Relevant knowledge from the knowledge base:\n\n{context}";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving RAG context");
            return string.Empty;
        }
    }

    private static string SanitizeQuery(string query)
    {
        // Extract meaningful words and format for MySQL BOOLEAN mode FULLTEXT search
        var stopWords = new HashSet<string> { "a", "an", "the", "is", "it", "in", "on", "at", "to", "for", "of", "and", "or", "what", "how", "why", "when", "where", "who" };
        var words = query.ToLower()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 2 && !stopWords.Contains(w))
            .Select(w => $"+{w}*");
        return string.Join(" ", words);
    }
}
