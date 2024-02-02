using FileshareBackend.Models;

namespace FileshareBackend.Services.Interfaces;

public interface IFileSystemService
{
    public FSEntryModel GetFromDirectory(string[] pathArr);
    public void MoveItem(string[] pathArr, string[] newPathArr, string name, bool overwrite);
}