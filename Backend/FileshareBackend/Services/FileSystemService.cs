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
    public FSEntryModel GetFromDirectory(string[] pathArr)
    {
        for (int i = 0; i < pathArr.Length; i++)
        {
            pathArr[i] = Utils.CleanString(pathArr[i]);
        }

        string path = "";
        try
        {
            path = string.Join('/', pathArr);
        }
        catch (Exception e)
        {
            
        }
        
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
            string[] split = files[i].Split('/');
            string name = split[split.Length - 1];
            FileModel file = new FileModel()
            {
                Name = name
            };
            
            entries.Files.Add(file);
        }

        return entries;

    }

    public void MoveFile(string[] pathArr, string[] newPathArr, string name, bool overwrite)
    {
        for (int i = 0; i < pathArr.Length; i++)
        {
            pathArr[i] = Utils.CleanString(pathArr[i]);
        }

        string path = "";
        try
        {
            path = string.Join('/', pathArr);
        }
        catch (Exception e)
        {
            
        }

        string newPath = "";
        for (int i = 0; i < pathArr.Length; i++)
        {
            newPathArr[i] = Utils.CleanString(newPathArr[i]);
        }
        try
        {
            newPath = string.Join('/', newPathArr);
        }
        catch (Exception e)
        {
            
        }

        newPath = Path.Join(_config["DirectoryRootPath"], newPath, name);
        path = Path.Join(_config["DirectoryRootPath"], path, name);
        if (!overwrite && File.Exists(newPath)) throw new FileSystemException("File exists in new location");
        File.Move(path, newPath, overwrite);
        
    }

    public void MoveDirectory(string[] pathArr, string[] newPathArr)
    {
        for (int i = 0; i < pathArr.Length; i++)
        {
            pathArr[i] = Utils.CleanString(pathArr[i]);
        }

        string path = "";
        try
        {
            path = string.Join('/', pathArr);
        }
        catch (Exception e)
        {
            
        }

        string newPath = "";
        for (int i = 0; i < newPathArr.Length; i++)
        {
            newPathArr[i] = Utils.CleanString(newPathArr[i]);
        }
        try
        {
            newPath = string.Join('/', newPathArr);
        }
        catch (Exception e)
        {
            
        }

        newPath = Path.Join(_config["DirectoryRootPath"], newPath);
        path = Path.Join(_config["DirectoryRootPath"], path);
        try
        {
            Directory.Move(path, newPath);
        }
        catch (IOException e)
        {
            throw new FileSystemException(e.Message);
        }
        
    }
    
    private int GetItemCount(string path)
    {
        return Directory.GetFileSystemEntries(path).Length;
    }
}