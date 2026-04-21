using Microsoft.EntityFrameworkCore;

namespace webshop_owp.Data.Base
{
    /// <summary>
    /// Represents a paginated collection of items, providing details about the current page, total pages, and navigation availability.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        /// <summary>
        /// Asynchronously creates a paginated list from an IQueryable source by counting the total items and retrieving only the items for the requested page.
        /// </summary>
        /// <param name="source">The queryable source of data.</param>
        /// <param name="pageIndex">The 1-based index of the page to retrieve.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A task that represents the asynchronous operation, returning a new instance of PaginatedList containing the requested data.</returns>
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
