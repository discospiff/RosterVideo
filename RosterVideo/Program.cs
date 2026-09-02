var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// Register in-memory roster store as a singleton
builder.Services.AddSingleton<RosterVideo.Services.IRosterStore, RosterVideo.Services.InMemoryRosterStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
