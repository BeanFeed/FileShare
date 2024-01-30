namespace FileshareBackend.Services.Interfaces;

public interface IFileSystemService
{
    public string[] GetFromDirectory(string path);
}