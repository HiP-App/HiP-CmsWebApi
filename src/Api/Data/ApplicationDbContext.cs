using BOL.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class ApplicationDbContext : CmsDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
    }
}
