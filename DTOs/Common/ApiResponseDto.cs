namespace MilkCollector.API.DTOs.Common
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ApiResponseDto<T> Ok(T data, string message = "Success")
            => new() { Success = true, Message = message, Data = data };

        // Making Fail generic to support all return types
        public static ApiResponseDto<T> Fail(string message)
            => new() { Success = false, Message = message, Data = default };
    }
}
