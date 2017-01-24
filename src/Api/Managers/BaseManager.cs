using Api.Data;

namespace Api.Managers
{
    public class BaseManager
    {
        protected readonly CmsDbContext dbContext;

        protected BaseManager(CmsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        protected static void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }
    }
}
