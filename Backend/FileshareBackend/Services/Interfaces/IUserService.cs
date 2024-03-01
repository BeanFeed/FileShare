using DAL.Entities;
using FileshareBackend.Models;

namespace FileshareBackend.Services.Interfaces;

public interface IUserService
{
    public Task<string> Register(UserLoginModel userInfo);
    public Task<string> Login(UserLoginModel userInfo);
    public bool OwnsFile(string[] pathArr, int id);
    public Task<User> GetMe(HttpContext ctx);
}