using FileshareBackend.Exceptions;
using FileshareBackend.Models;
using FileshareBackend.Services.Interfaces;

namespace FileshareBackend.Services;

public class FileSystemService : IFileSystemService
{
    private readonly IConfiguration _config;
    public FileSystemService(IConfiguration config)
    {
        _config = config;
    }
    public FSEntryModel GetFromDirectory(string path)
    {
        string fullPath = Path.Join(_config["DirectoryRootPath"], path);
        if (!Directory.Exists(fullPath))
            throw new FileSystemException($"{path} not Found");
        string[] directories = Directory.GetDirectories(fullPath);
        string[] files = Directory.GetFiles(fullPath);
        List<string> allList = new List<string>();
        FSEntryModel entries = new FSEntryModel();
        
        for (int i = 0; i < directories.Length; i++)
        {
            string[] folders = directories[i].Split('/');
            string name = folders[folders.Length - 1];
            DirectoryModel directory = new DirectoryModel()
            {
                Name = name,
                ItemCount = GetItemCount(directories[i])
            };
            entries.Directories.Add(directory);
        }

        for (int i = 0; i < files.Length; i++)
        {
            string[] split = directories[i].Split('/');
            string name = split[split.Length - 1];
            FileModel file = new FileModel()
            {
                Name = name
            };
            
            entries.Files.Add(file);
        }

        return entries;

    }

    private int GetItemCount(string path)
    {
        return Directory.GetFileSystemEntries(path).Length;
    }
}