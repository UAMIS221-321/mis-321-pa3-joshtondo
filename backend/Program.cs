using CryptoBot.Data;
using CryptoBot.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Connection String ────────────────────────────────────────────
static string GetConnectionString(IConfiguration config)
{
    // Heroku JawsDB MySQL (format: mysql://user:pass@host:port/db)
    var jawsDbUrl = Environment.GetEnvironmentVariable("JAWSDB_URL")
                    ?? Environment.GetEnvironmentVariable("CLEARDB_DATABASE_URL");

    if (!string.IsNullOrEmpty(jawsDbUrl))
    {
        try
        {
            var uri = new Uri(jawsDbUrl);
            var userInfo = uri.UserInfo.Split(':');
            var database = uri.AbsolutePath.TrimStart('/');
            return $"Server={uri.Host};Port={uri.Port};Database={database};User={userInfo[0]};Password={userInfo[1]};AllowPublicKeyRetrieval=true;SslMode=None;";
        }
        catch
        {
            // fall through to appsettings
        }
    }

    return config.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("No database connection string configured.");
}

var connectionString = GetConnectionString(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(3)));

// ── Services ─────────────────────────────────────────────────────
builder.Services.AddHttpClient<ICryptoApiService, CryptoApiService>(client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "CryptoBot/1.0");
    client.Timeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddHttpClient<IAnthropicService, AnthropicService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddScoped<IRagService, RagService>();
builder.Services.AddControllers();

// ── CORS ──────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// ── Database Init ─────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    DbInitializer.Seed(db);
}

// ── Middleware ────────────────────────────────────────────────────
app.UseCors();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// Bind to Heroku's dynamic PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Run($"http://0.0.0.0:{port}");
