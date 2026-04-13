using System.Text.Json;
using CryptoBot.Models;

namespace CryptoBot.Services;

public interface ICryptoApiService
{
    Task<string> GetCoinPriceAsync(string coinId);
    Task<string> GetMarketDataAsync(string coinId);
    Task<string> GetTrendingCoinsAsync();
}

public class CryptoApiService(HttpClient httpClient, ILogger<CryptoApiService> logger) : ICryptoApiService
{
    private const string BaseUrl = "https://api.coingecko.com/api/v3";

    public async Task<string> GetCoinPriceAsync(string coinId)
    {
        try
        {
            var url = $"{BaseUrl}/simple/price?ids={coinId}&vs_currencies=usd&include_24hr_change=true&include_market_cap=true";
            var response = await httpClient.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, double>>>(response);

            if (data == null || !data.ContainsKey(coinId))
                return $"Could not find price data for '{coinId}'. Make sure to use the CoinGecko ID (e.g., 'bitcoin', 'ethereum', 'solana').";

            var coin = data[coinId];
            var price = coin.GetValueOrDefault("usd");
            var change = coin.GetValueOrDefault("usd_24h_change");
            var mcap = coin.GetValueOrDefault("usd_market_cap");

            return JsonSerializer.Serialize(new
            {
                coin_id = coinId,
                price_usd = price,
                price_change_24h_percent = Math.Round(change, 2),
                market_cap_usd = mcap
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching price for {CoinId}", coinId);
            return $"Error fetching price for {coinId}: {ex.Message}";
        }
    }

    public async Task<string> GetMarketDataAsync(string coinId)
    {
        try
        {
            var url = $"{BaseUrl}/coins/{coinId}?localization=false&tickers=false&community_data=false&developer_data=false";
            var response = await httpClient.GetStringAsync(url);
            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            var marketData = root.GetProperty("market_data");
            var price = marketData.GetProperty("current_price").GetProperty("usd").GetDouble();
            var change24h = marketData.GetProperty("price_change_percentage_24h").GetDouble();
            var change7d = marketData.GetProperty("price_change_percentage_7d").GetDouble();
            var mcap = marketData.GetProperty("market_cap").GetProperty("usd").GetDouble();
            var volume = marketData.GetProperty("total_volume").GetProperty("usd").GetDouble();
            var high24h = marketData.GetProperty("high_24h").GetProperty("usd").GetDouble();
            var low24h = marketData.GetProperty("low_24h").GetProperty("usd").GetDouble();
            var ath = marketData.GetProperty("ath").GetProperty("usd").GetDouble();

            var name = root.GetProperty("name").GetString();
            var symbol = root.GetProperty("symbol").GetString()?.ToUpper();
            var rank = root.GetProperty("market_cap_rank").GetInt32();

            return JsonSerializer.Serialize(new
            {
                name,
                symbol,
                coin_id = coinId,
                market_cap_rank = rank,
                current_price_usd = price,
                price_change_24h_percent = Math.Round(change24h, 2),
                price_change_7d_percent = Math.Round(change7d, 2),
                market_cap_usd = mcap,
                total_volume_usd = volume,
                high_24h_usd = high24h,
                low_24h_usd = low24h,
                all_time_high_usd = ath
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching market data for {CoinId}", coinId);
            return $"Error fetching market data for {coinId}: {ex.Message}";
        }
    }

    public async Task<string> GetTrendingCoinsAsync()
    {
        try
        {
            var url = $"{BaseUrl}/search/trending";
            var response = await httpClient.GetStringAsync(url);
            using var doc = JsonDocument.Parse(response);
            var coins = doc.RootElement.GetProperty("coins");

            var result = new List<object>();
            foreach (var item in coins.EnumerateArray().Take(7))
            {
                var coinData = item.GetProperty("item");
                result.Add(new
                {
                    name = coinData.GetProperty("name").GetString(),
                    symbol = coinData.GetProperty("symbol").GetString(),
                    market_cap_rank = coinData.TryGetProperty("market_cap_rank", out var rank) ? rank.GetInt32() : 0,
                    coin_id = coinData.GetProperty("id").GetString()
                });
            }

            return JsonSerializer.Serialize(new { trending_coins = result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching trending coins");
            return $"Error fetching trending coins: {ex.Message}";
        }
    }
}
