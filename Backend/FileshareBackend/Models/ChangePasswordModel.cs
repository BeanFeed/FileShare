namespace FileshareBackend.Models;

public class ChangePasswordModel
{
    public string Username { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string Retype { get; set; }
    public bool? IsAdmin { get; set; } = false;
}