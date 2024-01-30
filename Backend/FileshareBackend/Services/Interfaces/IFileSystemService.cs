using FileshareBackend.Models;

namespace FileshareBackend.Services.Interfaces;

public interface IFileSystemService
{
    public FSEntryModel GetFromDirectory(string path);
}