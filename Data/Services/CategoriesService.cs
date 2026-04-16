using webshop_owp.Data.Base;
using webshop_owp.Models;

namespace webshop_owp.Data.Services
{
    /// <summary>
    /// Implements specific operations for managing product categories.
    /// </summary>
    public class CategoriesService : EntityBaseRepository<Category>, ICategoriesService
    {
        private readonly ILogger<CategoriesService> _logger;

        public CategoriesService(AppDbContext context, ILogger<CategoriesService> logger) : base(context) 
        { 
            _logger = logger;
        }
    }
}