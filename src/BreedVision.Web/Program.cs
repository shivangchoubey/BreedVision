using BreedVision.Web.Data;
using BreedVision.Web.Services.Implementations;
using BreedVision.Web.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Configuration with Automatic Failover
var dbProvider = builder.Configuration["DatabaseProvider"] ?? "Auto";
var sqlServerConn = builder.Configuration.GetConnectionString("SqlServerConnection");
var sqliteConn = builder.Configuration.GetConnectionString("SqliteConnection") ?? "Data Source=Data/breedvision.db";

bool useSqlServer = false;

if (dbProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase) || dbProvider.Equals("Auto", StringComparison.OrdinalIgnoreCase))
{
    if (!string.IsNullOrEmpty(sqlServerConn))
    {
        try
        {
            // Test SQL Server availability with short timeout
            var testBuilder = new SqlConnectionStringBuilder(sqlServerConn)
            {
                ConnectTimeout = 2
            };
            using var testConn = new SqlConnection(testBuilder.ConnectionString);
            testConn.Open();
            useSqlServer = true;
        }
        catch
        {
            useSqlServer = false;
        }
    }
}

if (useSqlServer && !string.IsNullOrEmpty(sqlServerConn))
{
    Console.WriteLine("[Database] Using Microsoft SQL Server database provider.");
    builder.Services.AddDbContext<BreedVisionDbContext>(options =>
        options.UseSqlServer(sqlServerConn));
}
else
{
    Console.WriteLine("[Database] Using SQLite database provider (portable local database).");
    var dataDir = Path.Combine(builder.Environment.ContentRootPath, "Data");
    if (!Directory.Exists(dataDir))
    {
        Directory.CreateDirectory(dataDir);
    }
    builder.Services.AddDbContext<BreedVisionDbContext>(options =>
        options.UseSqlite(sqliteConn));
}

// Register Application Services
builder.Services.AddSingleton<IBreedClassifierService, OnnxBreedClassifierService>();
builder.Services.AddScoped<IImageStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IBreedCatalogService, BreedCatalogService>();
builder.Services.AddScoped<IPredictionHistoryService, PredictionHistoryService>();

var app = builder.Build();

// Seed Database and Ensure Uploads Directory on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<BreedVisionDbContext>();

    try
    {
        var env = services.GetRequiredService<IWebHostEnvironment>();
        var uploadsDir = Path.Combine(env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsDir))
        {
            Directory.CreateDirectory(uploadsDir);
        }

        await DbInitializer.InitializeAsync(context, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database initialization or seeding.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
