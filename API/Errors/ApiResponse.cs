using System;

namespace API.Errors
{
    public class ApiResponse
    {
        public ApiResponse(int status, string message = null)
        {
            Status = status;
            Message = message ?? GetDefaultMessageForStatusCode(status);
        }
        public int Status { get; set; }
        public string Message { get; set; }
        private string GetDefaultMessageForStatusCode(int status)
        {
            return status switch
            {
                400 => "A bad request you have made",
                401 => "Authorized, you are not",
                404 => "Resource found, it was not",
                500 => "Errors are the path to the dark side, Errors lead to anger. Anger leads to hate. Hate leads to career change.",
                _ => null
            };
        }
    }
}