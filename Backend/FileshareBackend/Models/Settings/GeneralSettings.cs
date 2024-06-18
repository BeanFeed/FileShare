namespace FileshareBackend.Models.Settings;

public class GeneralSettings
{
    public string AllowedHosts { get; set; }
    public string DirectoryRootPath { get; set; }
    public string DatabaseConnectionString { get; set; }
}