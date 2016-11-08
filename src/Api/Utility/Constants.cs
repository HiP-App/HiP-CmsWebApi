using System.IO;

namespace Api.Utility
{
    public static class Constants
    {
        // No. of items to show in Paginated Result
        public const int PageSize = 50;

        public const string DefaultPircture = "default.jpg";

        public const string ProfilePictureFolder = "profilepictures";

        public static string ProfilePicturePath
        {
            get { return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ProfilePictureFolder); }
        }

        public const string AttatchmentFolder = "attatchments";

        public static string AttatchmentPath
        {
            get { return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", AttatchmentFolder); }
        }
    }
}
