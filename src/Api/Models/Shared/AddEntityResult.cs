namespace Api.Models
{
    public class AddEntityResult
    {
        public bool Success { get; set; }

        public object Value { get; set; }

        public string ErrorMessage { get; set; }
    }
}