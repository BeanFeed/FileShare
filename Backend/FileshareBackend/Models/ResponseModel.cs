namespace FileshareBackend.Models;

public class ResponseModel<T> where T : class
{
    public bool Success { get; set; }
    public T Message { get; set; }

    public ResponseModel(bool success, T message)
    {
        Success = success;
        Message = message;
    }
}