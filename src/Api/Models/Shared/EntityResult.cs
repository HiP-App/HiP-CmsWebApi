namespace PaderbornUniversity.SILab.Hip.CmsApi.Models
{
    public class EntityResult
    {
        public bool Success { get; set; }

        public object Value { get; set; }

        public string ErrorMessage { get; set; }

        public static EntityResult Successful() => Successful(null);

        public static EntityResult Successful(object value) => new EntityResult
        {
            Success = true,
            Value = value
        };

        public static EntityResult Error(string errorMessage) => new EntityResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}
