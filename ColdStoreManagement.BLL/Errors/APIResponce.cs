namespace ColdStoreManagement.BLL.Errors
{
    public class APIResponce(int StatusCode, string? message = null)
    {
        public int StatusCode { get; set; } = StatusCode;
        public string? Message { get; set; } = message ?? DefaultStatusCodeMessage(StatusCode);

        private static string DefaultStatusCodeMessage(int StatusCode)
        {
            return StatusCode switch
            {
                400 => "A bad request you have made",
                401 => "Authorized you have not",
                404 => "Resource Found it was not",
                500 => "Errors are the path to the dark side.  Errors lead to anger.   Anger leads to hate.  Hate leads to career change.",
                0 => "Some Thing Went Wrong",
                _ => throw new NotImplementedException()
            };
        }
    }
}
