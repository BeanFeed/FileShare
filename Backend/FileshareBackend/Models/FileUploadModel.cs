namespace FileshareBackend.Models;

public class FileUploadModel
{
    public IFormFile File { get; set; }
    public string[] Path { get; set; }
}