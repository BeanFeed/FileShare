namespace FileshareBackend.Models;

public class ResponseModel
{
    public bool Success { get; set; }
    public object Message { get; set; }

    public ResponseModel(bool success, object message)
    {
        Success = success;
        Message = message;
    }
}