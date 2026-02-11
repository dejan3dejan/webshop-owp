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
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

                context.Database.EnsureCreated();

                // Categories
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(new List<Category>()
                    {
                        new Category() { Name = "Smartphones", Description = "Latest mobile phones and devices" },
                        new Category() { Name = "Laptops", Description = "Powerful laptops for work and gaming" },
                        new Category() { Name = "Accessories", Description = "Headphones, chargers and more" }
                    });
                    context.SaveChanges();
                }

                // Products
                if (!context.Products.Any())
                {
                    context.Products.AddRange(new List<Product>()
                    {
                        new Product()
                        {
                            Name = "iPhone 15 Pro",
                            Description = "Apple's latest flagship with Titanium design",
                            Price = 999.99,
                            ImageUrl = "https://cdsassets.apple.com/live/7WUAS350/images/tech-specs/iphone-15-pro-max.png",
                            CategoryId = 1 // Smartphones
                        },
                        new Product()
                        {
                            Name = "MacBook Air M2",
                            Description = "Supercharged by M2 chip, thin and light",
                            Price = 1199.00,
                            ImageUrl = "https://images.unsplash.com/photo-1611186871348-b1ce696e52c9",
                            CategoryId = 2 // Laptops
                        },
                        new Product()
                        {
                            Name = "Sony WH-1000XM5",
                            Description = "Industry leading noise canceling headphones",
                            Price = 349.50,
                            ImageUrl = "https://backend.gigatron.rs/media/catalog/product/cache/d62e1a0582bf7257bddc609f302ce89c/s/o/sony-slusalice-wh-1000xm5-6669f98412c3c.jpg",
                            CategoryId = 3 // Accessories
                        }
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