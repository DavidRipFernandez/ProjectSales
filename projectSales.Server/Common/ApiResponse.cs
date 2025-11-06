namespace projectSales.Server.Common;

public class ApiResponse<T>
{
    public string? Message { get; set; }
    public string? TechnicalMessage { get; set; }
    public T? Data { get; set; }
    public int StatusCode { get; set; }
    public bool Success { get; set; }

    public static ApiResponse<T> Ok(T data, string? msg = null)
        => new() { Data = data, Message = msg ?? "Operación exitosa", StatusCode = 200, Success = true };

    public static ApiResponse<T> Fail(string msg, string? tech = null, int code = 400)
        => new() { Message = msg, TechnicalMessage = tech, StatusCode = code, Success = false };
}
