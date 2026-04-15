using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webshop_owp.Data;
using webshop_owp.Data.Cart;
using webshop_owp.Data.Services;
using webshop_owp.Models;
using Serilog;
using webshop_owp.Infrastructure.Middleware;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/webshop-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

// Authentication and Authorization configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// Services configuration
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped(sp => ShoppingCart.GetCart(sp));

builder.Services.AddMemoryCache();
builder.Services.AddSession();


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ViewTrackingMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using var scope = app.Services.CreateScope();
var initializerLogger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbInitializer>>();
try 
{
    await AppDbInitializer.SeedDataAsync(app);
}
catch (Exception ex)
{
    initializerLogger.LogError(ex, "An error occurred while seeding the database.");
}

app.Run();