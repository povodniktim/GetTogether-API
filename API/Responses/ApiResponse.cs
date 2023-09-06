namespace API.Responses
{
    public abstract class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public dynamic[]? Errors { get; set; }
    }

    public class SuccessResponse<T> : ApiResponse<T>
    {
        public SuccessResponse(T data, string message = "Success")
        {
            Success = true;
            Message = message;
            Data = data;
            Errors = null;
        }
    }

    public class ErrorResponse<T> : ApiResponse<T>
    {
        public ErrorResponse(dynamic[] errorMessages, string message = "Failed", T data = default(T))
        {
            Success = false;
            Message = message;
            Data = data;
            Errors = errorMessages;
        }
    }
}