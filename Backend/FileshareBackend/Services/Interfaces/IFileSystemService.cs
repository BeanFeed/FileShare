using DAL.Entities;
using FileshareBackend.Models;

namespace FileshareBackend.Services.Interfaces;

public interface IFileSystemService
{
    public FSEntryModel GetFromDirectory(string[] pathArr, User? user);
    public FSEntryModel GetFromDirectory(string path);
    public void MoveFile(string[] pathArr, string[] newPathArr, string name, User user, bool overwrite);
    public void MoveDirectory(string[] pathArr, string[] newPathArr, User user);
    public void RenameItem(string[] pathArr, string newName, User user);

    public void DeleteItem(string[] pathArr, User user);
    public void UploadFile(string[] pathArr, IFormFile file, User user);
    public void ChangeItemVisibility(string[] pathArr, string visibility);
    public bool CanDownload(string[] pathArr, User? user);
}