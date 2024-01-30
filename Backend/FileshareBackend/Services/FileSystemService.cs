using FileshareBackend.Exceptions;
using FileshareBackend.Services.Interfaces;

namespace FileshareBackend.Services;

public class FileSystemService : IFileSystemService
{
    private readonly IConfiguration _config;
    public FileSystemService(IConfiguration config)
    {
        _config = config;
    }
    public string[] GetFromDirectory(string path)
    {
        string fullPath = Path.Join(_config["DirectoryRootPath"], path);
        if (!Directory.Exists(fullPath))
            throw new FileSystemException($"{path} not Found");
        string[] directories = Directory.GetDirectories(fullPath);
        string[] files = Directory.GetFiles(fullPath);
        List<string> allList = new List<string>();
        
        for (int i = 0; i < directories.Length; i++)
        {
            allList.Add(directories[i]);
        }

        for (int i = 0; i < files.Length; i++)
        {
            allList.Add(files[i]);
        }

        return allList.ToArray();
    }
}