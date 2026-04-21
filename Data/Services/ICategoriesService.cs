using webshop_owp.Data.Base;
using webshop_owp.Models;

namespace webshop_owp.Data.Services
{
    /// <summary>
    /// Provides specific operations for managing product categories.
    /// </summary>
    public interface ICategoriesService : IEntityBaseRepository<Category>
    {
    }
}