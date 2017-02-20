using System.Collections.Generic;
using System.Linq;

namespace Api.Utility
{
    public class PagedResult<TEntity>
    {
        public PagedResult(IEnumerable<TEntity> list, int page, int pageSize, int total)
        {
            Items = list;

            Metadata = new Pagination {
                ItemsCount = Items.Count(),
                TotalItems = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (total + pageSize - 1) / pageSize
            };
        }
        
        public IEnumerable<TEntity> Items { get; set; }

        public Pagination Metadata { get; set; }
    }

    public class Pagination
    {
        public int ItemsCount { get; set; }

        public int TotalItems { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }
    }
}
