namespace API.Errors
{
    public class ApiResponse
    {
        public ApiResponse(int status, string message = null, string path = null)
        {
            Status = status;
            Message = message ?? GetDefaultMessageForStatusCode(status);
            Path = path;
        }
        public int Status { get; set; }
        public string Message { get; set; }
        public string Path { get; set; }
        private static string GetDefaultMessageForStatusCode(int status)
        {
            return status switch
            {
                400 => "Bad Request",
                401 => "Unauthorized action",
                403 => "Forbidden action",
                404 => "Resource not found",
                500 => "Something went wrong, please try again",
                _ => null
            };
        }
    }
}