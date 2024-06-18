using System.Text;
using System.Text.Json;
using DAL.Entities;
using FileshareBackend.Exceptions;
using FileshareBackend.Models;
using FileshareBackend.Models.Settings;
using FileshareBackend.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace FileshareBackend.Services;

public class FileSystemService : IFileSystemService
{
    private readonly GeneralSettings _generalSettings;
    public FileSystemService(IOptions<GeneralSettings> generalSettings)
    {
        _generalSettings = generalSettings.Value;
    }

    public FileSystemService()
    {
        _generalSettings = new GeneralSettings();
    }

    public FSEntryModel GetFromDirectory(string[] pathArr, User? user)
    {
        for (int i = 0; i < pathArr.Length; i++)
        {
            
            pathArr[i] = Utils.CleanString(pathArr[i]);
        }

        string path = "";
        //string parentPath = "";
        try
        {
            path = string.Join('/', pathArr);
            /*
            List<string> ppArr = pathArr.ToList();
            ppArr.RemoveAt(ppArr.Count-1);
            parentPath = string.Join('/', ppArr);
            */
        }
        catch (Exception e)
        {
            throw new FileSystemException("Failed to locate directory");
        }
        
        string fullPath = Path.Join(_generalSettings.DirectoryRootPath, path);
        /*
        if (!Directory.Exists(fullPath))
        {
            if (!Directory.Exists(Path.Join(_generalSettings.DirectoryRootPath, parentPath)) || !File.Exists(fullPath))
            {
                throw new FileSystemException($"{path} not Found");
            }
            else
            {
                fullPath = Path.Join(_generalSettings.DirectoryRootPath, parentPath);
            }
                
        }
        */
        if (!Directory.Exists(fullPath)) throw new FileSystemException($"{path} not Found");
        string[] directories = Directory.GetDirectories(fullPath).OrderBy(f => f).ToArray();
        string[] files = Directory.GetFiles(fullPath).Where(x => !x.EndsWith("fsmeta.json")).OrderBy(f => f).ToArray();
        
        
        List<string> allList = new List<string>();
        FSEntryModel entries = new FSEntryModel();
        MetaFileModel meta = null!;
        try
        {
            meta = GetMeta(fullPath);
        }
        catch (FileSystemException e)
        {
            throw;
        }

        entries.CanEdit = user != null && meta.Owner == user.Username;

        if (!entries.CanEdit && meta.AnyCanUpload) entries.CanEdit = true;
            
        if (meta.Visibility == "private" && (user != null && user.Username != meta.Owner))
            throw new FileSystemException("Unauthorized");
        for (int i = 0; i < directories.Length; i++)
        {
            string[] folders = directories[i].Split('/');
            string name = folders[^1];
            var dirMeta = meta.Directories.FirstOrDefault(x => x.Name == name);
            if((dirMeta?.Visibility == "private" || dirMeta?.Visibility == "unlisted") && (user != null && user.Username != dirMeta.Owner)) continue;
            DirectoryModel directory = new DirectoryModel()
            {
                Name = name,
                ItemCount = GetItemCount(directories[i]),
                CanEdit = user?.Username == dirMeta?.Owner
            };
            entries.Directories.Add(directory);
        }

        for (int i = 0; i < files.Length; i++)
        {
            string[] split = files[i].Split('/');
            string name = split[split.Length - 1];
            var fileMeta = meta.Files.First(x => x.Name == name);
            if ((fileMeta.Visibility == "private" || fileMeta.Visibility == "unlisted") &&
                (user == null || user.Username != fileMeta.Owner))
            {
                continue;
            }
            FileModel file = new FileModel()
            {
                Name = name,
                CanEdit = user?.Username == fileMeta.Owner
            };
            
            entries.Files.Add(file);
        }

        return entries;

    }
    
    //Only used in Utils
    public FSEntryModel GetFromDirectory(string path)
    {
        
        string[] directories = Directory.GetDirectories(path).OrderBy(f => f).ToArray();
        string[] files = Directory.GetFiles(path).OrderBy(f => f).ToArray();
        FSEntryModel entries = new FSEntryModel();
        
        for (int i = 0; i < directories.Length; i++)
        {
            string[] folders = directories[i].Split('/');
            string name = folders[^1];
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
            string name = split[^1];
            FileModel file = new FileModel()
            {
                Name = name
            };
            
            entries.Files.Add(file);
        }

        return entries;

    }

    public ItemPropertiesResModel GetItemProperties(string[] pathArr, bool isDirectory, User? user)
    {
        string fullPath = Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', pathArr));
        if (isDirectory)
        {
            if (GetDirectoryOwner(pathArr) == user.Username)
            {
                MetaFileModel meta;

                try
                {
                    meta = GetMeta(Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', pathArr)));
                }
                catch (FileSystemException e)
                {
                    throw;
                }
                
                var res = new ItemPropertiesResModel()
                {
                    Name = pathArr[^1],
                    Visibility = GetItemVisibility(pathArr, isDirectory),
                    AnyoneCanUpload = meta.AnyCanUpload 
                };
                return res;
            }
            else
            {
                throw new FileSystemException("Unauthorized");
            }
        }
        else
        {
            if (GetFileOwner(pathArr) == user.Username)
            {
                var res = new ItemPropertiesResModel()
                {
                    Name = pathArr[^1],
                    Visibility = GetItemVisibility(pathArr, isDirectory)
                };
                return res;
            }
            else
            {
                throw new FileSystemException("Unauthorized");
            }
        }

    }
    
    public void MoveFile(string[] pathArr, string[] newPathArr, string name, User user, bool overwrite)
    {
        if (user == null) throw new FileSystemException("Unauthorized");
        for (int i = 0; i < pathArr.Length; i++)
        {
            pathArr[i] = Utils.CleanString(pathArr[i]);
        }
        if (!OwnsItem(pathArr.Append(name).ToArray(), user.Username, true)) throw new FileSystemException("Unauthorized");

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

        newPath = Path.Join(_generalSettings.DirectoryRootPath, newPath, name);
        
        path = Path.Join(_generalSettings.DirectoryRootPath, path, name);
        if (!overwrite && File.Exists(newPath)) throw new FileSystemException("File exists in new location");
        File.Move(path, newPath, overwrite);
        RemoveMeta(pathArr.Append(name).ToArray(), true);
        AddMeta(newPathArr.Append(name).ToArray(), true, user.Username);
    }

    public void MoveDirectory(string[] pathArr, string[] newPathArr, User user)
    {
        for (int i = 0; i < pathArr.Length; i++)
        {
            pathArr[i] = Utils.CleanString(pathArr[i]);
        }

        if (!OwnsItem(pathArr, user.Username, false)) throw new FileSystemException("Unauthorized");
        
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

        
        newPath = Path.Join(_generalSettings.DirectoryRootPath, newPath);
        path = Path.Join(_generalSettings.DirectoryRootPath, path);
        try
        {
            Directory.Move(path, newPath);
            RemoveMeta(pathArr, false);
            AddMeta(newPathArr, false, user.Username);
        }
        catch (IOException e)
        {
            throw new FileSystemException(e.Message);
        }
        
    }

    public void RenameItem(string[] pathArr, string newName, User user)
    {
        for (int i = 0; i < pathArr.Length; i++)
        {
            pathArr[i] = Utils.CleanString(pathArr[i]);
        }
        if (pathArr[^1] == "fsmeta.json") throw new FileSystemException("Unauthorized");
        if (newName == "fsmeta.json") throw new FileSystemException("Unauthorized");
        
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
        string[] oldPR = (string[])pathArr.Clone();
        try
        {
            pathArr[pathArr.Length - 1] = newName;
            newPath = string.Join('/', pathArr);
        }
        catch
        {
            throw new FileSystemException("Failed to locate directory");
        }

        oldPath = Path.Join(_generalSettings.DirectoryRootPath, oldPath);
        newPath = Path.Join(_generalSettings.DirectoryRootPath, newPath);
        if (Directory.Exists(oldPath) && !OwnsItem(pathArr, user.Username, false))
            throw new FileSystemException("Unauthorized");
        if (File.Exists(oldPath) && !OwnsItem(pathArr, user.Username, true))
            throw new FileSystemException("Unauthorized");
        bool isFile = !Directory.Exists(oldPath);
        try
        {
            Directory.Move(oldPath, newPath);
            RemoveMeta(oldPR, isFile);
            AddMeta(pathArr, isFile, user.Username);
        }
        catch (IOException e)
        {
            throw new FileSystemException(e.Message);
        }


    }

    

    public void DeleteItem(string[] pathArr, User user)
    {
        for (int i = 0; i < pathArr.Length; i++)
        {
            pathArr[i] = Utils.CleanString(pathArr[i]);
        }

        if (pathArr[^1] == "fsmeta.json") throw new FileSystemException("Unauthorized");
        string path = "";
        try
        {
            
            path = Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', pathArr));
        }
        catch
        {
            throw new FileSystemException("Failed to locate item");
        }
        
        if (Directory.Exists(path))
        {
            if (!OwnsItem(pathArr, user.Username, false)) throw new FileSystemException("Unauthorized");
            if (Directory.GetFileSystemEntries(path).Length > 1 || !Directory.GetFileSystemEntries(path)[0].EndsWith("fsmeta.json")) throw new FileSystemException("Directory not empty");
            File.Delete(Directory.GetFileSystemEntries(path)[0]);
            Directory.Delete(path);
            RemoveMeta(pathArr, false);
        } 
        else if (File.Exists(path))
        {
            if (!OwnsItem(pathArr, user.Username, true)) throw new FileSystemException("Unauthorized");
            File.Delete(path);
            RemoveMeta(pathArr, true);
        }
        else
        {
            throw new FileSystemException("File Not Found");
        }
    }

    public void UploadFile(string[] pathArr, IFormFile file, User user)
    {
        for (int i = 0; i < pathArr.Length; i++)
        {
            pathArr[i] = Utils.CleanString(pathArr[i]);
        }

        var parentDir = pathArr.Take(pathArr.Length - 1).ToArray();
        MetaFileModel meta;

        try
        {
            meta = GetMeta(Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', parentDir)));
        }
        catch (FileSystemException e)
        {
            throw;
        }
        

        if (meta.Owner != user.Username && !meta.AnyCanUpload) throw new FileSystemException("Unauthorized");
        
        if (file.FileName == "fsmeta.json") throw new FileSystemException("Unauthorized");
        string path = "";
        try
        {
            
            path = Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', pathArr));
        }
        catch
        {
            throw new FileSystemException("Failed to locate directory");
        }

        if (File.Exists(Path.Join(path, file.FileName))) throw new FileSystemException("File already exists");
        AddMeta(Path.Join(path, file.FileName).Split('/'), true, user.Username);
        var stream = File.OpenWrite(Path.Join(path, file.FileName));
        
        file.CopyTo(stream);
        
        stream.Close();
    }

    public void ChangeItemVisibility(string[] pathArr, string visibility)
    {
        var itemName = pathArr[^1];
        var parentDir = pathArr.Take(pathArr.Length - 1).ToArray();
        MetaFileModel meta;
        try
        {
            meta = GetMeta(Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', parentDir)));
        }
        catch (FileSystemException e)
        {
            throw;
        }
            

        if (Directory.Exists(Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', pathArr))))
        {
            ItemMeta dirMeta = meta.Directories.First(x => x.Name == itemName);
            dirMeta.Visibility = visibility;
            
        } else if (File.Exists(Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', pathArr))))
        {
            ItemMeta fileMeta = meta.Files.First(x => x.Name == itemName);
            fileMeta.Visibility = visibility;
        }
        else
        {
            throw new FileSystemException("Couldn't find item");
        }
        
        File.WriteAllText(Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', parentDir), "fsmeta.json"),
            string.Empty);
        var stream = File.OpenWrite(Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', parentDir), "fsmeta.json"));
        stream.Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(meta, new JsonSerializerOptions(){WriteIndented = true})));
        stream.Close();
        stream.Dispose();
    }

    public void ChangePublicUploading(string[] pathArr, bool anyoneCanUpload)
    {
        var path = Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', pathArr));
        MetaFileModel meta;

        try
        {
            meta = GetMeta(path);
        }
        catch (FileSystemException e)
        {
            throw new FileSystemException(e.Message);
        }

        meta.AnyCanUpload = anyoneCanUpload;
        
        File.WriteAllText(Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', pathArr), "fsmeta.json"),
            string.Empty);
        var stream = File.OpenWrite(Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', pathArr), "fsmeta.json"));
        stream.Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(meta, new JsonSerializerOptions(){WriteIndented = true})));
        stream.Close();
        stream.Dispose();
        
    }

    public bool CanDownload(string[] pathArr, User? user)
    {
        List<string> t = pathArr.ToList();
        t.RemoveAt(t.Count - 1);
        string[] newArr = t.ToArray();
        
        MetaFileModel meta;

        try
        {
            meta = GetMeta(Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', newArr)));;
        }
        catch (FileSystemException e)
        {
            throw new FileSystemException(e.Message);
        }
        
        var fileMeta = meta.Files.First(x => x.Name == pathArr[^1]);
        if (fileMeta.Owner == user?.Username || fileMeta.Visibility != "private") return true;
        return false;
    }

    private bool OwnsItem(string[] pathArr, string Owner, bool isFile)
    {
        string[] parentPath = pathArr.Take(pathArr.Length - 1).ToArray();


        MetaFileModel meta;

        try
        {
            meta = GetMeta(Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', parentPath)));
        }
        catch (FileSystemException e)
        {
            throw;
        }
        
        if (isFile)
        {
            return meta.Files.First(x => x.Name == pathArr[^1]).Owner == Owner;
        }
        return meta.Directories.First(x => x.Name == pathArr[^1]).Owner == Owner;
    }

    public MetaFileModel GetMeta(string path)
    {
        var rawText = File.ReadAllText(Path.Join(path,"fsmeta.json"));
        try
        {
            return JsonSerializer.Deserialize<MetaFileModel>(rawText);
        }
        catch (Exception e)
        {
            throw new FileSystemException("Server-Side file corrupted");
        }
        
    }

    private void AddMeta(string[] itemPath, bool isFile, string Owner)
    {
        string[] parentPath = itemPath.Take(itemPath.Length - 1).ToArray();

        MetaFileModel meta;

        try
        {
            meta = GetMeta(string.Join('/', parentPath));
        }
        catch (FileSystemException e)
        {
            throw;
        }
        
        var fileMeta = new ItemMeta()
        {
            Name = itemPath[^1],
            Owner = Owner,
            Visibility = "public"
        };
        if (isFile)
        {
            meta.Files = meta.Files.Append(fileMeta).ToArray();
        }
        else
        {
            meta.Directories = meta.Directories.Append(fileMeta).ToArray();
        }

        File.WriteAllText( Path.Join(string.Join('/', parentPath), "fsmeta.json"), string.Empty);
        var stream = File.OpenWrite(Path.Join(string.Join('/', parentPath), "fsmeta.json"));
        stream.Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(meta, new JsonSerializerOptions(){WriteIndented = true})));
        stream.Close();
        stream.Dispose();
    }

    private void RemoveMeta(string[] itemPath, bool isFile)
    {
        string[] parentPath = itemPath.Take(itemPath.Length - 1).ToArray();

        MetaFileModel meta;

        try
        {
            meta = GetMeta(Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', parentPath)));
        }
        catch (FileSystemException e)
        {
            throw;
        }
        
        if (isFile)
        {
            meta.Files = meta.Files.Where(x => x.Name != itemPath[^1]).ToArray();
        }
        else
        {
            meta.Directories = meta.Directories.Where(x => x.Name != itemPath[^1]).ToArray();
        }
        File.WriteAllText(Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', parentPath), "fsmeta.json"),
            string.Empty);
        var stream = File.OpenWrite(Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', parentPath), "fsmeta.json"));
        stream.Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(meta, new JsonSerializerOptions(){WriteIndented = true})));
        stream.Close();
        stream.Dispose();
    }

    private string GetFileOwner(string[] pathArr)
    {
        var parentDir = pathArr.Take(pathArr.Length - 1).ToArray();
        var path = Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', parentDir));
        MetaFileModel meta;

        try
        {
            meta = GetMeta(path);
        }
        catch (FileSystemException e)
        {
            throw;
        }

        var file = meta.Files.First(x => x.Name == pathArr[^1]);
        return file.Owner;
    }

    private string GetDirectoryOwner(string[] pathArr)
    {
        var path = Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', pathArr));

        MetaFileModel meta;

        try
        {
            meta = GetMeta(path);
        }
        catch (FileSystemException e)
        {
            throw;
        }
        
        return meta.Owner;
    }

    private string GetItemVisibility(string[] pathArr, bool isDirectory)
    {
        if (isDirectory)
        {
            var parentDir = pathArr.Take(pathArr.Length - 1).ToArray();
            var path = Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', parentDir));
            
            MetaFileModel meta;

            try
            {
                meta = GetMeta(path);
            }
            catch (FileSystemException e)
            {
                throw;
            }
            
            var directory = meta.Directories.First(x => x.Name == pathArr[^1]);
            return directory.Visibility;
        }
        else //Resume with get item property endpoint
        {
            var parentDir = pathArr.Take(pathArr.Length - 1).ToArray();
            var path = Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', parentDir));
            MetaFileModel meta;

            try
            {
                meta = GetMeta(path);
            }
            catch (FileSystemException e)
            {
                throw;
            }
            
            var file = meta.Files.First(x => x.Name == pathArr[^1]);
            return file.Visibility;
        }
    }
    private int GetItemCount(string path)
    {
        return Directory.GetFileSystemEntries(path).Length - 1;
    }
    
    
    
}