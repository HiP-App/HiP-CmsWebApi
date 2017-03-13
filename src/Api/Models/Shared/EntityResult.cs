namespace Api.Models
{
    public class EntityResult
    {
        public bool Success { get; set; }

        public object Value { get; set; }

        public string ErrorMessage { get; set; }

        public static EntityResult Successfull()
        {
            return Successfull(null);
        }

        public static EntityResult Successfull(object value)
        {
            return new EntityResult() { Success = true, Value = value };
        }

        public static EntityResult Error(string errorMessage)
        {
            return new EntityResult() { Success = false, ErrorMessage = errorMessage };
        }
    }
}