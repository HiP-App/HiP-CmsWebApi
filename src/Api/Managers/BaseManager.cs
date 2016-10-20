using BOL.Data;

namespace BLL.Managers
{
    public class BaseManager
    {
        protected readonly CmsDbContext dbContext;

        public BaseManager(CmsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
