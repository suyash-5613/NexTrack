using Microsoft.EntityFrameworkCore;
using NexTrack.Data;
using NexTrack.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

// Database configuration
// Uses InMemory by default for easy demo. Switch to SQL Server by setting
// "UseInMemory": false in appsettings.json and providing a connection string.
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemory", true);

if (useInMemory)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("NexTrackDb"));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Session support (for simple auth)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Items}/{action=Index}/{id?}");

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();

    // For InMemory database, seed data doesn't auto-apply via HasData, so do it manually
    if (useInMemory && !context.Items.Any())
    {
        context.Items.AddRange(
            new Item { Id = 1, Name = "Raw Material Batch A", Weight = 1000m, ParentId = null, Status = "pending", CreatedAt = DateTime.UtcNow },
            new Item { Id = 2, Name = "Raw Material Batch B", Weight = 500m, ParentId = null, Status = "processed", CreatedAt = DateTime.UtcNow },
            new Item { Id = 3, Name = "Component X", Weight = 200m, ParentId = 2, Status = "pending", CreatedAt = DateTime.UtcNow },
            new Item { Id = 4, Name = "Component Y", Weight = 300m, ParentId = 2, Status = "pending", CreatedAt = DateTime.UtcNow },
            new Item { Id = 5, Name = "Raw Material Batch C", Weight = 1500m, ParentId = null, Status = "pending", CreatedAt = DateTime.UtcNow }
        );
        context.SaveChanges();
    }
}

app.Run();
