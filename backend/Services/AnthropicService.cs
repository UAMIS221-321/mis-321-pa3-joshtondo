using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CryptoBot.Models;

namespace CryptoBot.Services;

public interface IAnthropicService
{
    Task<(string AssistantMessage, List<ToolCallResult> ToolResults)> ChatAsync(
        string userMessage,
        List<ChatMessage> history,
        string ragContext);
}

public class AnthropicService(
    HttpClient httpClient,
    IConfiguration config,
    ICryptoApiService cryptoApi,
    ILogger<AnthropicService> logger) : IAnthropicService
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly List<object> Tools = new()
    {
        new
        {
            name = "get_crypto_price",
            description = "Get the current USD price, 24h change, and market cap for a cryptocurrency using its CoinGecko ID.",
            input_schema = new
            {
                type = "object",
                properties = new
                {
                    coin_id = new
                    {
                        type = "string",
                        description = "CoinGecko coin ID in lowercase (e.g., 'bitcoin', 'ethereum', 'solana', 'cardano', 'dogecoin')"
                    }
                },
                required = new[] { "coin_id" }
            }
        },
        new
        {
            name = "get_coin_market_data",
            description = "Get detailed market data for a cryptocurrency including price, market cap, 24h & 7d change, volume, ATH, and 24h high/low.",
            input_schema = new
            {
                type = "object",
                properties = new
                {
                    coin_id = new
                    {
                        type = "string",
                        description = "CoinGecko coin ID in lowercase (e.g., 'bitcoin', 'ethereum', 'solana')"
                    }
                },
                required = new[] { "coin_id" }
            }
        },
        new
        {
            name = "get_trending_coins",
            description = "Get a list of currently trending cryptocurrencies on CoinGecko based on search volume.",
            input_schema = new
            {
                type = "object",
                properties = new { },
                required = Array.Empty<string>()
            }
        }
    };

    public async Task<(string AssistantMessage, List<ToolCallResult> ToolResults)> ChatAsync(
        string userMessage,
        List<ChatMessage> history,
        string ragContext)
    {
        var apiKey = config["ANTHROPIC_API_KEY"] ?? Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")
            ?? throw new InvalidOperationException("ANTHROPIC_API_KEY is not configured.");

        var systemPrompt = BuildSystemPrompt(ragContext);
        var messages = BuildMessages(history, userMessage);
        var toolResults = new List<ToolCallResult>();

        var maxIterations = 5;
        for (var i = 0; i < maxIterations; i++)
        {
            var request = new
            {
                model = "claude-haiku-4-5-20251001",
                max_tokens = 1024,
                system = systemPrompt,
                tools = Tools,
                messages
            };

            var json = JsonSerializer.Serialize(request, JsonOpts);
            var httpReq = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            httpReq.Headers.Add("x-api-key", apiKey);
            httpReq.Headers.Add("anthropic-version", "2023-06-01");

            var httpResp = await httpClient.SendAsync(httpReq);
            var respJson = await httpResp.Content.ReadAsStringAsync();

            if (!httpResp.IsSuccessStatusCode)
            {
                logger.LogError("Anthropic API error {Status}: {Body}", httpResp.StatusCode, respJson);
                return ("Sorry, I encountered an error contacting the AI service. Please try again.", toolResults);
            }

            var response = JsonSerializer.Deserialize<AnthropicApiResponse>(respJson, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNameCaseInsensitive = true
            });

            if (response == null)
                return ("Sorry, I received an unexpected response. Please try again.", toolResults);

            if (response.StopReason != "tool_use")
            {
                var text = response.Content
                    .Where(b => b.Type == "text")
                    .Select(b => b.Text)
                    .FirstOrDefault() ?? string.Empty;
                return (text, toolResults);
            }

            // Handle tool use
            messages.Add(new { role = "assistant", content = (object)response.Content });

            var toolResultBlocks = new List<object>();
            foreach (var block in response.Content.Where(b => b.Type == "tool_use"))
            {
                var toolResult = await ExecuteToolAsync(block.Name ?? "", block.Input);
                toolResults.Add(new ToolCallResult(block.Name ?? "", toolResult));
                toolResultBlocks.Add(new
                {
                    type = "tool_result",
                    tool_use_id = block.Id,
                    content = toolResult
                });
            }

            messages.Add(new { role = "user", content = (object)toolResultBlocks });
        }

        return ("I was unable to complete the request after multiple attempts.", toolResults);
    }

    private static string BuildSystemPrompt(string ragContext)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are CryptoSage, an expert cryptocurrency assistant. You help users understand crypto markets, blockchain technology, DeFi, NFTs, trading strategies, and the broader crypto ecosystem.");
        sb.AppendLine();
        sb.AppendLine("You have access to real-time crypto price data via tools. Use them when users ask about current prices, market data, or trending coins.");
        sb.AppendLine();
        sb.AppendLine("Guidelines:");
        sb.AppendLine("- Be accurate, helpful, and explain complex concepts clearly");
        sb.AppendLine("- Always note that crypto investments carry risk and this is not financial advice");
        sb.AppendLine("- Use the knowledge base context below when relevant");
        sb.AppendLine("- Format numbers clearly (e.g., $45,230.50 for prices, $1.2T for large numbers)");

        if (!string.IsNullOrWhiteSpace(ragContext))
        {
            sb.AppendLine();
            sb.AppendLine("--- KNOWLEDGE BASE CONTEXT ---");
            sb.AppendLine(ragContext);
            sb.AppendLine("--- END KNOWLEDGE BASE CONTEXT ---");
        }

        return sb.ToString();
    }

    private static List<object> BuildMessages(List<ChatMessage> history, string userMessage)
    {
        var messages = new List<object>();
        foreach (var msg in history)
        {
            messages.Add(new { role = msg.Role, content = (object)msg.Content });
        }
        messages.Add(new { role = "user", content = (object)userMessage });
        return messages;
    }

    private async Task<string> ExecuteToolAsync(string toolName, Dictionary<string, JsonElement>? input)
    {
        return toolName switch
        {
            "get_crypto_price" => await cryptoApi.GetCoinPriceAsync(
                input?.GetValueOrDefault("coin_id").GetString() ?? "bitcoin"),
            "get_coin_market_data" => await cryptoApi.GetMarketDataAsync(
                input?.GetValueOrDefault("coin_id").GetString() ?? "bitcoin"),
            "get_trending_coins" => await cryptoApi.GetTrendingCoinsAsync(),
            _ => $"Unknown tool: {toolName}"
        };
    }

    // Internal deserialization models
    private class AnthropicApiResponse
    {
        [JsonPropertyName("stop_reason")]
        public string StopReason { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public List<ResponseBlock> Content { get; set; } = [];
    }

    private class ResponseBlock
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("input")]
        public Dictionary<string, JsonElement>? Input { get; set; }
    }
}
