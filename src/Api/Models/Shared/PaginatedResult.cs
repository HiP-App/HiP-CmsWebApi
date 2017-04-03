using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Utility
{
    public class PagedResult<TEntity>
    {
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public PagedResult(IEnumerable<TEntity> list, int total)
        {
            Items = list;
            Metadata = new Pagination
            {
                ItemsCount = total,
                TotalItems = total,
                Page = 1,
                PageSize = total,
                TotalPages = 1
            };
        }

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
