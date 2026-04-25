using webshop_owp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using webshop_owp.Data.Static;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace webshop_owp.Data
{
    public class AppDbInitializer
    {
        // Old synchronous seed method kept for backward compatibility if ever called elsewhere
        public static void Seed(IApplicationBuilder applicationBuilder) { }

        public static async Task SeedDataAsync(IApplicationBuilder applicationBuilder)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
            var logger = serviceScope.ServiceProvider.GetService<ILogger<AppDbInitializer>>();
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                logger.LogInformation("Starting database initialization...");
                context.Database.EnsureCreated();

                // 1. Seed Roles
                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                // 2. Seed Admin User
                string adminUserEmail = "admin@techshop.com";
                if (await userManager.FindByEmailAsync(adminUserEmail) == null)
                {
                    var newAdminUser = new ApplicationUser() { FullName = "Admin User", UserName = "admin-user", Email = adminUserEmail, EmailConfirmed = true };
                    await userManager.CreateAsync(newAdminUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                // 3. Seed 30 ML Synthetic Users
                var userIds = new List<string>();
                for (int i = 1; i <= 30; i++)
                {
                    string email = $"user{i}@techshop.com";
                    var user = await userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new ApplicationUser() { FullName = $"ML User {i}", UserName = $"ml-user-{i}", Email = email, EmailConfirmed = true };
                        await userManager.CreateAsync(user, "Coding@1234?");
                        await userManager.AddToRoleAsync(user, UserRoles.User);
                    }
                    userIds.Add(user.Id);
                }

                // Check if data already exists
                if (context.Categories.Any() || context.Products.Any())
                {
                    logger.LogInformation("Database already contains data, skipping seed.");
                    return;
                }

                // 4. Seed Categories
                var categories = new List<Category>
                {
                    new Category { Name = "Electronics", Description = "Home appliances and displays" }, // 1
                    new Category { Name = "Audio", Description = "Headphones, speakers and soundbars" }, // 2
                    new Category { Name = "Gaming", Description = "Consoles, games and VR" }, // 3
                    new Category { Name = "Smartphones", Description = "Latest mobile devices" }, // 4
                    new Category { Name = "Laptops", Description = "Work and gaming portables" }, // 5
                    new Category { Name = "Accessories", Description = "Cables, mice, keyboards" }, // 6
                    new Category { Name = "Wearables", Description = "Smartwatches and fitness trackers" }, // 7
                    new Category { Name = "Cameras", Description = "DSLR, mirrorless and action cams" } // 8
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();

                // 5. Seed Products (60 products)
                var random = new Random(1337); // Seeded for reproducibility
                var products = new List<Product>();
                int pId = 1;

                string[] cats = { "Electronics", "Audio", "Gaming", "Smartphones", "Laptops", "Accessories", "Wearables", "Cameras" };
                var catMap = context.Categories.ToDictionary(c => c.Name, c => c.Id);

                void AddP(string name, string cat, double price, string tags)
                {
                    products.Add(new Product { 
                        Name = name, 
                        Description = $"High quality {name} for best experience.", 
                        Price = price, 
                        CategoryId = catMap[cat],
                        StockAmount = random.Next(10, 100),
                        DiscountPercentage = random.NextDouble() < 0.3 ? random.Next(5, 30) : 0,
                        ImageUrl = "https://picsum.photos/400/300",
                        Tags = tags
                    });
                }

                // Electronics
                AddP("Samsung 65\" QLED TV", "Electronics", 1200, "tv,qled,samsung,4k,display");
                AddP("LG OLED 55\"", "Electronics", 1500, "tv,oled,lg,4k,premium");
                AddP("Roku Ultra", "Electronics", 99, "streaming,roku,4k,wifi");
                AddP("Apple TV 4K", "Electronics", 129, "streaming,apple,4k,smart");
                AddP("Philips Hue Kit", "Electronics", 199, "smart-home,lighting,philips,wifi");
                AddP("Nest Thermostat", "Electronics", 129, "smart-home,google,climate,wifi");
                AddP("Roomba i7+", "Electronics", 600, "home,vacuum,robot,cleaning,smart");
                // Audio
                AddP("Sony WH-1000XM5", "Audio", 348, "headphones,wireless,noise-canceling,sony");
                AddP("AirPods Pro 2", "Audio", 249, "earbuds,wireless,apple,noise-canceling");
                AddP("Bose QC45", "Audio", 329, "headphones,wireless,bose,comfort");
                AddP("Sennheiser Momentum 4", "Audio", 350, "headphones,wireless,audiophile");
                AddP("Sonos Roam", "Audio", 179, "speaker,bluetooth,sonos,portable");
                AddP("JBL Flip 6", "Audio", 129, "speaker,bluetooth,jbl,waterproof");
                AddP("Yamaha Soundbar", "Audio", 200, "speaker,home-theater,yamaha,tv");
                AddP("Audio-Technica M50x", "Audio", 149, "headphones,wired,studio,professional");
                // Gaming
                AddP("PlayStation 5", "Gaming", 499, "console,sony,ps5,next-gen");
                AddP("Xbox Series X", "Gaming", 499, "console,microsoft,xbox,next-gen");
                AddP("Nintendo Switch OLED", "Gaming", 349, "console,nintendo,portable");
                AddP("Steam Deck", "Gaming", 399, "console,pc,valve,portable");
                AddP("Oculus Quest 2", "Gaming", 299, "vr,meta,headset,wireless");
                AddP("DualSense Controller", "Gaming", 69, "controller,sony,ps5,accessory");
                AddP("Xbox Wireless Controller", "Gaming", 59, "controller,microsoft,xbox,pc");
                AddP("PS5 Pulse 3D", "Gaming", 99, "headset,gaming,sony,ps5");
                // Smartphones
                AddP("iPhone 15 Pro", "Smartphones", 999, "phone,apple,ios,camera,flagship");
                AddP("Galaxy S24 Ultra", "Smartphones", 1199, "phone,samsung,android,stylus");
                AddP("Pixel 8 Pro", "Smartphones", 899, "phone,google,android,ai");
                AddP("OnePlus 12", "Smartphones", 799, "phone,oneplus,android,fast-charge");
                AddP("iPhone 14", "Smartphones", 699, "phone,apple,ios");
                AddP("Galaxy A54", "Smartphones", 449, "phone,samsung,android,mid-range");
                AddP("Pixel 7a", "Smartphones", 499, "phone,google,android,budget");
                AddP("Sony Xperia 1 V", "Smartphones", 1199, "phone,sony,android,creator");
                // Laptops
                AddP("MacBook Pro 16", "Laptops", 2499, "laptop,apple,mac,professional");
                AddP("MacBook Air M2", "Laptops", 1199, "laptop,apple,mac,lightweight");
                AddP("Dell XPS 15", "Laptops", 1499, "laptop,dell,windows,premium");
                AddP("Lenovo ThinkPad X1", "Laptops", 1399, "laptop,lenovo,windows,business");
                AddP("Asus ROG Zephyrus", "Laptops", 1799, "laptop,asus,gaming,windows");
                AddP("Razer Blade 15", "Laptops", 1999, "laptop,razer,gaming,premium");
                AddP("HP Spectre x360", "Laptops", 1299, "laptop,hp,windows,2-in-1");
                // Accessories
                AddP("Logitech MX Master 3S", "Accessories", 99, "mouse,logitech,wireless,productivity");
                AddP("Keychron K2", "Accessories", 89, "keyboard,mechanical,wireless,mac");
                AddP("Anker USB-C Hub", "Accessories", 49, "hub,usb-c,anker,connectivity");
                AddP("SanDisk 1TB SSD", "Accessories", 109, "storage,ssd,portable,sandisk");
                AddP("Magic Keyboard", "Accessories", 99, "keyboard,apple,wireless");
                AddP("Razer DeathAdder", "Accessories", 49, "mouse,razer,gaming,wired");
                AddP("SteelSeries Apex Pro", "Accessories", 199, "keyboard,gaming,mechanical,rgb");
                AddP("Webcam C920", "Accessories", 79, "webcam,logitech,video,streaming");
                // Wearables
                AddP("Apple Watch Series 9", "Wearables", 399, "watch,apple,smartwatch,health");
                AddP("Apple Watch Ultra 2", "Wearables", 799, "watch,apple,rugged,gps");
                AddP("Galaxy Watch 6", "Wearables", 299, "watch,samsung,smartwatch,android");
                AddP("Garmin Fenix 7", "Wearables", 699, "watch,garmin,fitness,gps");
                AddP("Fitbit Charge 6", "Wearables", 149, "tracker,fitbit,health,fitness");
                AddP("Oura Ring Gen3", "Wearables", 299, "ring,oura,sleep,health");
                AddP("Pixel Watch 2", "Wearables", 349, "watch,google,smartwatch,fitbit");
                // Cameras
                AddP("Sony A7 IV", "Cameras", 2499, "camera,sony,mirrorless,full-frame");
                AddP("Canon EOS R6", "Cameras", 2299, "camera,canon,mirrorless,video");
                AddP("Fujifilm X-T5", "Cameras", 1699, "camera,fujifilm,mirrorless,aps-c");
                AddP("GoPro HERO12", "Cameras", 399, "camera,gopro,action,waterproof");
                AddP("DJI Osmo Action 4", "Cameras", 349, "camera,dji,action,video");
                AddP("Insta360 X3", "Cameras", 449, "camera,insta360,360,video");
                AddP("Nikon Z6 II", "Cameras", 1999, "camera,nikon,mirrorless,photography");

                context.Products.AddRange(products);
                await context.SaveChangesAsync();

                var allProducts = context.Products.ToList();
                var audioProducts = allProducts.Where(p => p.CategoryId == catMap["Audio"]).ToList();
                var gamingProducts = allProducts.Where(p => p.CategoryId == catMap["Gaming"]).ToList();
                var smartphoneProducts = allProducts.Where(p => p.CategoryId == catMap["Smartphones"]).ToList();
                var laptopProducts = allProducts.Where(p => p.CategoryId == catMap["Laptops"]).ToList();
                var accessoryProducts = allProducts.Where(p => p.CategoryId == catMap["Accessories"]).ToList();
                var wearableProducts = allProducts.Where(p => p.CategoryId == catMap["Wearables"]).ToList();
                var cameraProducts = allProducts.Where(p => p.CategoryId == catMap["Cameras"]).ToList();

                // 6. Seed 300 Orders
                var orders = new List<Order>();
                for (int i = 0; i < 300; i++)
                {
                    string uId = userIds[random.Next(userIds.Count)];
                    var createdAt = DateTime.UtcNow.AddDays(-random.Next(1, 540));
                    var status = random.NextDouble() < 0.85 ? OrderStatus.Completed : OrderStatus.Cancelled;

                    var order = new Order
                    {
                        UserId = uId,
                        Email = $"user@techshop.com", // dummy
                        FullName = "ML User",
                        Address = "123 Tech Lane",
                        City = "Techville",
                        TotalAmount = 0, // will calc later
                        DiscountAmount = 0,
                        CreatedAt = createdAt,
                        Status = status,
                        OrderItems = new List<OrderItem>()
                    };

                    int numItems = random.Next(1, 5); // 1 to 4 items
                    var selectedProducts = new List<Product>();

                    // Pick base item
                    var baseProduct = allProducts[random.Next(allProducts.Count)];
                    selectedProducts.Add(baseProduct);

                    // Co-purchase patterns encoded
                    for (int j = 1; j < numItems; j++)
                    {
                        if (baseProduct.CategoryId == catMap["Audio"] && random.NextDouble() < 0.6)
                            selectedProducts.Add(audioProducts[random.Next(audioProducts.Count)]); // Audio + Audio
                        else if (baseProduct.CategoryId == catMap["Gaming"] && random.NextDouble() < 0.7)
                            selectedProducts.Add(accessoryProducts[random.Next(accessoryProducts.Count)]); // Gaming + Accessories
                        else if (baseProduct.CategoryId == catMap["Smartphones"] && random.NextDouble() < 0.5)
                            selectedProducts.Add(wearableProducts[random.Next(wearableProducts.Count)]); // Smartphones + Wearables
                        else if (baseProduct.CategoryId == catMap["Cameras"] && random.NextDouble() < 0.4)
                            selectedProducts.Add(laptopProducts[random.Next(laptopProducts.Count)]); // Cameras + Laptops
                        else if (baseProduct.CategoryId == catMap["Laptops"] && random.NextDouble() < 0.6)
                            selectedProducts.Add(accessoryProducts[random.Next(accessoryProducts.Count)]); // Laptops + Accessories
                        else
                            selectedProducts.Add(allProducts[random.Next(allProducts.Count)]); // Random
                    }

                    selectedProducts = selectedProducts.Distinct().ToList(); // Avoid duplicate products in same order
                    
                    double total = 0;
                    foreach (var sp in selectedProducts)
                    {
                        int amount = random.Next(1, 3);
                        order.OrderItems.Add(new OrderItem
                        {
                            ProductId = sp.Id,
                            Amount = amount,
                            Price = sp.Price
                        });
                        total += sp.Price * amount;
                    }
                    order.TotalAmount = total;
                    orders.Add(order);
                }
                context.Orders.AddRange(orders);
                await context.SaveChangesAsync();

                // 7. Seed 3000 Product Views
                var productViews = new List<ProductView>();
                var orderedProductsByUser = context.OrderItems
                    .Include(oi => oi.Order)
                    .Where(oi => oi.Order.UserId != null)
                    .GroupBy(oi => oi.Order.UserId!)
                    .ToDictionary(g => g.Key, g => g.Select(oi => oi.ProductId).Distinct().ToList());

                for (int i = 0; i < 3000; i++)
                {
                    bool isAnonymous = random.NextDouble() < 0.2;
                    string? uId = isAnonymous ? null : userIds[random.Next(userIds.Count)];
                    int pIdToView = allProducts[random.Next(allProducts.Count)].Id;

                    // "Products with more views should also have more orders (realistic signal)"
                    // To do this, we intentionally view products the user has ordered, or popular products
                    if (!isAnonymous && orderedProductsByUser.TryGetValue(uId!, out var userProducts) && userProducts.Any() && random.NextDouble() < 0.5)
                    {
                        pIdToView = userProducts[random.Next(userProducts.Count)];
                    }
                    else if (random.NextDouble() < 0.3) // popular items get viewed more
                    {
                        // Apple track
                        var popular = allProducts.Where(p => p.Name.Contains("Apple") || p.Name.Contains("Sony")).ToList();
                        if (popular.Any()) pIdToView = popular[random.Next(popular.Count)].Id;
                    }

                    var viewedAt = DateTime.UtcNow.AddDays(-random.Next(1, 540)).AddHours(random.Next(-12, 12));

                    productViews.Add(new ProductView
                    {
                        ProductId = pIdToView,
                        UserId = uId,
                        SessionId = Guid.NewGuid().ToString(),
                        ViewedAt = viewedAt,
                        DurationSeconds = random.Next(10, 300)
                    });
                }
                context.ProductViews.AddRange(productViews);
                await context.SaveChangesAsync();

                logger.LogInformation("Successfully initialized ML-ready database seed.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to seed ML database.");
            }
        }
    }
}
