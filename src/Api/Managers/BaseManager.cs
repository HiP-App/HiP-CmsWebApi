using Api.Data;

namespace Api.Managers
{
    public class BaseManager
    {
        protected readonly CmsDbContext dbContext;

        public BaseManager(CmsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        protected void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }
    }
}
