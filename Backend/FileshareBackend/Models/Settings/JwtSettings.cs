namespace FileshareBackend.Models.Settings;

public class JwtSettings
{
    public string Key { get; set; }
    public string RKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Secure { get; set; }
    public string SSM { get; set; }
}