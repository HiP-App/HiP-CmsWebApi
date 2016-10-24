using System.IO;

namespace Api.Utility
{
    public static class Constants
    {
        // No. of items to show in Paginated Result
        public const int PageSize = 50;

        public static string ProfilePictureFolder
        {
            get { return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profilepictures"); }
        }

        public static string AttatchmentFolder
        {
            get { return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "attatchments"); }
        }
    }
}
