using FileshareBackend.Models;

namespace FileshareBackend.Services.Interfaces;

public interface IFileSystemService
{
    public FSEntryModel GetFromDirectory(string[] pathArr);
    public void MoveFile(string[] pathArr, string[] newPathArr, string name, bool overwrite);
    public void MoveDirectory(string[] pathArr, string[] newPathArr);
    public void RenameItem(string[] pathArr, string newName);

    public void DeleteItem(string[] pathArr);
    public void UploadFile(string[] pathArr, IFormFile file);
}