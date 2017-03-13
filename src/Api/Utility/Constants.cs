using System.IO;

namespace Api.Utility
{
    public static class Constants
    {
        // No. of items to show in Paginated Result
        public const int PageSize = 50;

        public const string DefaultPircture = "default.jpg";

        public const string ProfilePictureFolder = "profilepictures";

        public static string ProfilePicturePath => AbsolutePath(ProfilePictureFolder);

        public const string AttatchmentFolder = "attatchments";

        public static string AttatchmentPath => AbsolutePath(AttatchmentFolder);


        private static string AbsolutePath(string item)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item);
        }
    }
}
