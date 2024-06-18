namespace FileshareBackend.Models;

//This class is for the request, not response
public class ItemPropertiesReqModel
{
    public string[] pathArr { get; set; }
    public bool isDirectory { get; set; }
}