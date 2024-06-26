using DAL.Entities;
using FileshareBackend.Models;

namespace FileshareBackend.Services.Interfaces;

public interface IUserService
{
    public Task<string[]> Register(UserLoginModel userInfo);
    public Task<string[]> Login(UserLoginModel userInfo);
    public Task ChangePassword(ChangePasswordModel passwordModel);
    public Task<string[]> Refresh(string rToken);
    public Task<User> GetMe(string ctx);
}