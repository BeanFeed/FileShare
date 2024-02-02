using FileshareBackend.Models;

namespace FileshareBackend.Services.Interfaces;

public interface IFileSystemService
{
    public FSEntryModel GetFromDirectory(string[] pathArr);
    public void MoveFile(string[] pathArr, string[] newPathArr, string name, bool overwrite);
    public void MoveDirectory(string[] pathArr, string[] newPathArr);
}