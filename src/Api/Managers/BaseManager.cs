using Api.Data;

namespace Api.Managers
{
    public class BaseManager
    {
        protected readonly CmsDbContext DbContext;

        protected BaseManager(CmsDbContext dbContext)
        {
            DbContext = dbContext;
        }


        protected static void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }
    }
}
