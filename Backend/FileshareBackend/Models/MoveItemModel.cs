namespace FileshareBackend.Models;

public class MoveItemModel
{
    public string[] oldPath { get; set; }
    public string[] newPath { get; set; }
    public string? itemName { get; set; }
    public bool overwrite { get; set; }
}