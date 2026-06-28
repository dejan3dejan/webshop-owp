using Microsoft.EntityFrameworkCore;
using webshop_owp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using webshop_owp.Data.Static;

namespace webshop_owp.Data
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

                context.Database.Migrate();

                // Categories
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(new List<Category>()
                    {
                        new Category() { Name = "Smartphones", Description = "Latest mobile phones and devices" },
                        new Category() { Name = "Laptops", Description = "Powerful laptops for work and gaming" },
                        new Category() { Name = "Accessories", Description = "Headphones, chargers and more" },
                        new Category() { Name = "Monitors", Description = "Crystal clear displays for work and play" },
                        new Category() { Name = "Gaming", Description = "Consoles, controllers and gaming gear" }
                    });
                    context.SaveChanges();
                }

                // Products
                if (!context.Products.Any())
                {
                    context.Products.AddRange(new List<Product>()
                    {
                        new Product() { Name = "iPhone 15 Pro", Description = "Apple's latest flagship with Titanium design", Price = 999.99m, ImageUrl = "https://picsum.photos/seed/iphone/400/300", CategoryId = 1, StockAmount = 10 },
                        new Product() { Name = "Samsung Galaxy S24", Description = "AI-powered smartphone with amazing camera", Price = 899.00m, ImageUrl = "https://picsum.photos/seed/samsung/400/300", CategoryId = 1, StockAmount = 12 },
                        new Product() { Name = "Google Pixel 8", Description = "The best of Google with pure Android", Price = 699.00m, ImageUrl = "https://picsum.photos/seed/pixel/400/300", CategoryId = 1, StockAmount = 8 },
                        
                        new Product() { Name = "MacBook Air M2", Description = "Supercharged by M2 chip, thin and light", Price = 1199.00m, ImageUrl = "https://picsum.photos/seed/macbook/400/300", CategoryId = 2, StockAmount = 5 },
                        new Product() { Name = "Dell XPS 15", Description = "Performance meets elegance in this 15-inch laptop", Price = 1499.00m, ImageUrl = "https://picsum.photos/seed/dell/400/300", CategoryId = 2, StockAmount = 7 },
                        new Product() { Name = "ASUS ROG Zephyrus", Description = "Top tier gaming performance in a portable form", Price = 1799.99m, ImageUrl = "https://picsum.photos/seed/asus/400/300", CategoryId = 2, StockAmount = 3 },
                        
                        new Product() { Name = "Sony WH-1000XM5", Description = "Industry leading noise canceling headphones", Price = 349.50m, ImageUrl = "https://picsum.photos/seed/sony/400/300", CategoryId = 3, StockAmount = 15 },
                        new Product() { Name = "Logitech MX Master 3S", Description = "The ultimate mouse for productivity and comfort", Price = 99.00m, ImageUrl = "https://picsum.photos/seed/logitech/400/300", CategoryId = 3, StockAmount = 20 },
                        new Product() { Name = "Keychron K2", Description = "Mechanical keyboard with hot-swappable switches", Price = 89.00m, ImageUrl = "https://picsum.photos/seed/keyboard/400/300", CategoryId = 3, StockAmount = 10 },
                        
                        new Product() { Name = "LG UltraGear 27\"", Description = "144Hz Gaming monitor with G-Sync support", Price = 299.99m, ImageUrl = "https://picsum.photos/seed/lgmon/400/300", CategoryId = 4, StockAmount = 6 },
                        new Product() { Name = "Dell UltraSharp 32\"", Description = "4K USB-C Hub Monitor for professional work", Price = 749.00m, ImageUrl = "https://picsum.photos/seed/dellmon/400/300", CategoryId = 4, StockAmount = 4 },
                        
                        new Product() { Name = "PlayStation 5", Description = "Experience lightning-fast loading and immersive gaming", Price = 499.99m, ImageUrl = "https://picsum.photos/seed/ps5/400/300", CategoryId = 5, StockAmount = 10 },
                        new Product() { Name = "Xbox Series X", Description = "The fastest, most powerful Xbox ever", Price = 499.99m, ImageUrl = "https://picsum.photos/seed/xbox/400/300", CategoryId = 5, StockAmount = 10 },
                        new Product() { Name = "Nintendo Switch OLED", Description = "Vibrant 7-inch OLED screen for gaming on the go", Price = 349.00m, ImageUrl = "https://picsum.photos/seed/switch/400/300", CategoryId = 5, StockAmount = 12 }
                    });
                    context.SaveChanges();
                }

                // Coupons
                if (!context.Coupons.Any())
                {
                    context.Coupons.AddRange(new List<Coupon>()
                    {
                        new Coupon() { Code = "TECH10", DiscountPercentage = 10 },
                        new Coupon() { Code = "WELCOME20", DiscountPercentage = 20 },
                        new Coupon() { Code = "FREESHIP", DiscountPercentage = 5 }
                    });
                    context.SaveChanges();
                }
            }
        }

        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                //Users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                string adminUserEmail = "admin@techshop.com";

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new ApplicationUser()
                    {
                        FullName = "Admin User",
                        UserName = "admin-user",
                        Email = adminUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newAdminUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                string appUserEmail = "user@techshop.com";
                var appUser = await userManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var newAppUser = new ApplicationUser()
                    {
                        FullName = "Application User",
                        UserName = "app-user",
                        Email = appUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newAppUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
                }
            }
        }
    }
}
