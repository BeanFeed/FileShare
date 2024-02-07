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
            throw new FileSystemException("Failed to locate directory");
        }
        
        string fullPath = Path.Join(_config["DirectoryRootPath"], path);
        if (!Directory.Exists(fullPath))
            throw new FileSystemException($"{path} not Found");
        string[] directories = Directory.GetDirectories(fullPath).OrderBy(f => f).ToArray();
        string[] files = Directory.GetFiles(fullPath).OrderBy(f => f).ToArray();
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
            throw new FileSystemException("Failed to locate directory");
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
            throw new FileSystemException("Failed to locate directory");
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
            throw new FileSystemException("Failed to locate directory");
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
            throw new FileSystemException("Failed to locate directory");
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

    public void RenameItem(string[] pathArr, string newName)
    {
        for (int i = 0; i < pathArr.Length; i++)
        {
            pathArr[i] = Utils.CleanString(pathArr[i]);
        }

        string oldPath = "";
        try
        {
            oldPath = string.Join('/', pathArr);
        }
        catch
        {
            throw new FileSystemException("Failed to locate directory");
        }

        string newPath = "";
        try
        {
            pathArr[pathArr.Length - 1] = newName;
            newPath = string.Join('/', pathArr);
        }
        catch
        {
            throw new FileSystemException("Failed to locate directory");
        }

        oldPath = Path.Join(_config["DirectoryRootPath"], oldPath);
        newPath = Path.Join(_config["DirectoryRootPath"], newPath);
        try
        {
            Directory.Move(oldPath, newPath);
        }
        catch (IOException e)
        {
            throw new FileSystemException(e.Message);
        }


    }

    public void DeleteItem(string[] pathArr)
    {
        string path = "";Path.Join(_config["DirectoryRootPath"], string.Join('/', pathArr));
        try
        {
            
            path = Path.Join(_config["DirectoryRootPath"], string.Join('/', pathArr));
        }
        catch
        {
            throw new FileSystemException("Failed to locate item");
        }

        if (Directory.Exists(path))
        {
            if (Directory.GetDirectories(path).Length != 0) throw new FileSystemException("Directory not empty");
            
            Directory.Delete(path);
        } 
        else if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            throw new FileSystemException("File Not Found");
        }
    }
    
    private int GetItemCount(string path)
    {
        return Directory.GetFileSystemEntries(path).Length;
    }
    
    
}