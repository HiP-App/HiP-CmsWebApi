using PaderbornUniversity.SILab.Hip.CmsApi.Data;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Managers
{
    public class BaseManager
    {
        protected readonly CmsDbContext DbContext;

        protected BaseManager(CmsDbContext dbContext) => DbContext = dbContext;
    }
}
