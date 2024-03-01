namespace FileshareBackend.Models;

public class FSEntryModel
{
    public List<DirectoryModel> Directories { get; set; } = new List<DirectoryModel>();
    public List<FileModel> Files { get; set; } = new List<FileModel>();
    public bool CanEdit { get; set; }
}