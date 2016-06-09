using BOL.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class CmsDbContext : DbContext
    {
        public CmsDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Member> Members { get; set; }
    }
}
