namespace CryptoBot.Models;

// --- Request / Response DTOs ---

public record ChatRequest(string SessionId, string Message);

public record ChatResponse(
    string SessionId,
    string Message,
    string Role,
    List<ToolCallResult>? ToolResults
);

public record ToolCallResult(string ToolName, string Result);

public record NewSessionResponse(string SessionId, string Title);

// --- Anthropic API models ---

public class AnthropicRequest
{
    public string model { get; set; } = "claude-haiku-4-5-20251001";
    public int max_tokens { get; set; } = 1024;
    public string? system { get; set; }
    public List<AnthropicMessage> messages { get; set; } = [];
    public List<AnthropicTool>? tools { get; set; }
}

public class AnthropicMessage
{
    public string role { get; set; } = string.Empty;
    public object content { get; set; } = string.Empty; // string or List<ContentBlock>
}

public class AnthropicTool
{
    public string name { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public object input_schema { get; set; } = new();
}

public class AnthropicResponse
{
    public string id { get; set; } = string.Empty;
    public string stop_reason { get; set; } = string.Empty;
    public List<ContentBlock> content { get; set; } = [];
    public AnthropicUsage? usage { get; set; }
    public string? error { get; set; }
}

public class ContentBlock
{
    public string type { get; set; } = string.Empty;
    public string? text { get; set; }
    public string? id { get; set; }
    public string? name { get; set; }
    public Dictionary<string, object>? input { get; set; }
}

public class AnthropicUsage
{
    public int input_tokens { get; set; }
    public int output_tokens { get; set; }
}

// --- CoinGecko models ---

public class CoinPrice
{
    public string Id { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double CurrentPrice { get; set; }
    public double? MarketCap { get; set; }
    public double? PriceChangePercentage24h { get; set; }
    public double? TotalVolume { get; set; }
}

public class TrendingCoin
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public int MarketCapRank { get; set; }
}
