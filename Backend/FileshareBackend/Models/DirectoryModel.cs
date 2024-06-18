namespace FileshareBackend.Models;

public class DirectoryModel
{
    public string Name { get; set; }
    public int ItemCount { get; set; }
    public bool CanEdit { get; set; }
}