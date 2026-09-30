using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using RosterVideo.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// Register in-memory roster store by default
builder.Services.AddSingleton<RosterVideo.Services.IRosterStore, RosterVideo.Services.InMemoryRosterStore>();

// Configuration and detection
var useAzureDbSetting = builder.Configuration["UseAzureDatabase"];
var useAzureDbExplicit = !string.IsNullOrEmpty(useAzureDbSetting) && bool.TryParse(useAzureDbSetting, out var parsed) && parsed;
var isRunningOnAzure = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));

// Normalize connection string from MYSQLCONNSTR_localdb if present
string? rawAzureConn = Environment.GetEnvironmentVariable("MYSQLCONNSTR_localdb") ?? builder.Configuration["MYSQLCONNSTR_localdb"];
string? normalizedConn = null;
if (!string.IsNullOrEmpty(rawAzureConn))
{
    normalizedConn = NormalizeAzureInAppConnString(rawAzureConn);
}

// Decide whether to enable DB mode
var enableDb = useAzureDbExplicit || (!useAzureDbExplicit && isRunningOnAzure);
if (enableDb && !string.IsNullOrEmpty(normalizedConn))
{
    // Register DbContext and DbRosterStore
    builder.Services.AddDbContext<RosterDbContext>(opts =>
        opts.UseMySql(normalizedConn, ServerVersion.AutoDetect(normalizedConn)));

    builder.Services.AddScoped<RosterVideo.Services.IRosterStore, RosterVideo.Services.DbRosterStore>();

    // Optionally apply migrations automatically when running in Azure and MIGRATIONS_AUTOAPPLY=true
    var autoApplySetting = builder.Configuration["MIGRATIONS_AUTOAPPLY"] ?? Environment.GetEnvironmentVariable("MIGRATIONS_AUTOAPPLY");
    var autoApply = true;
    if (!string.IsNullOrEmpty(autoApplySetting) && !bool.TryParse(autoApplySetting, out var autoParsed))
    {
        autoApply = autoParsed;
    }

    builder.Services.AddSingleton(new DbStartupOptions { AutoMigrate = autoApply });
}

var app = builder.Build();

// Apply migrations if configured
if (app.Services.GetService(typeof(DbStartupOptions)) is DbStartupOptions options && options.AutoMigrate)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RosterDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Applying database migrations...");
        db.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetService<ILogger<Program>>();
        logger?.LogError(ex, "An error occurred while applying migrations.");
        // Swallow or rethrow depending on desired behavior; here we log and continue.
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

// Minimal API endpoint for JSON feed
app.MapGet("/api/roster", (RosterVideo.Services.IRosterStore store) =>
{
    var entries = store.GetAll()
        .OrderByDescending(e => e.CreatedAt)
        .ToArray();
    return Results.Json(entries);
}).WithName("RosterJson");

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

static string? NormalizeAzureInAppConnString(string raw)
{
    if (string.IsNullOrWhiteSpace(raw)) return null;

    // Parse semicolon-separated key=value parts
    var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    var parts = raw.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    foreach (var p in parts)
    {
        var idx = p.IndexOf('=');
        if (idx > 0)
        {
            var k = p.Substring(0, idx).Trim();
            var v = p.Substring(idx + 1).Trim();
            dict[k] = v;
        }
    }

    dict.TryGetValue("Database", out var database);
    dict.TryGetValue("Data Source", out var dataSource);
    dict.TryGetValue("DataSource", out var dsAlt);
    dict.TryGetValue("User Id", out var userId);
    dict.TryGetValue("UserId", out var userAlt);
    dict.TryGetValue("Password", out var password);

    if (string.IsNullOrEmpty(dataSource) && !string.IsNullOrEmpty(dsAlt)) dataSource = dsAlt;
    if (string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(userAlt)) userId = userAlt;

    string host = "";
    string port = "";
    if (!string.IsNullOrEmpty(dataSource))
    {
        // Data Source may be in form tcp:127.0.0.1,52137 or 127.0.0.1,52137 or 127.0.0.1:52137
        var ds = dataSource;
        if (ds.StartsWith("tcp:", StringComparison.OrdinalIgnoreCase)) ds = ds.Substring(4);
        // split by comma or colon
        var m = Regex.Match(ds, @"^(?<host>[^,:]+)(?:[:,](?<port>\d+))?$");
        if (m.Success)
        {
            host = m.Groups["host"].Value;
            port = m.Groups["port"].Success ? m.Groups["port"].Value : "";
        }
    }

    if (string.IsNullOrEmpty(host)) return null; // cannot normalize

    var sb = new System.Text.StringBuilder();
    sb.Append($"Server={host};");
    if (!string.IsNullOrEmpty(port)) sb.Append($"Port={port};");
    if (!string.IsNullOrEmpty(database)) sb.Append($"Database={database};");
    if (!string.IsNullOrEmpty(userId)) sb.Append($"User={userId};");
    if (!string.IsNullOrEmpty(password)) sb.Append($"Password={password};");

    return sb.ToString();
}

internal class DbStartupOptions
{
    public bool AutoMigrate { get; set; }
}
