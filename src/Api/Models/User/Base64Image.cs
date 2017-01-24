namespace Api.Models.User
{
    public class Base64Image
    {
        public Base64Image(string base64)
        {
            Base64 = base64;
        }

        public string Base64 { get; set; }
    }
}
