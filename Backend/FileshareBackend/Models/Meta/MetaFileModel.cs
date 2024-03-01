namespace FileshareBackend.Models;

public class MetaFileModel
{
    public string Type { get; set; }
    public string Owner { get; set; }
    public string Visibility { get; set; }
    public ItemMeta[] Files { get; set; }
    public ItemMeta[] Directories { get; set; }
}