using webshop_owp.Data.Base;
using webshop_owp.Models;

namespace webshop_owp.Data.Services
{
    public class CategoriesService : EntityBaseRepository<Category>, ICategoriesService
    {
        public CategoriesService(AppDbContext context) : base(context) { }
    }
}