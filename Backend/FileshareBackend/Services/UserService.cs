using DAL.Context;
using DAL.Entities;
using FileshareBackend.Exceptions;
using FileshareBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FileshareBackend.Services.Interfaces;

public class UserService : IUserService
{
    private readonly FileShareContext _fsContext;
    private readonly IConfiguration _config;
    private readonly IJwtService _jwtService;
    
    public UserService(FileShareContext context, IConfiguration config, IJwtService jwtService)
    {
        _fsContext = context;
        _config = config;
        _jwtService = jwtService;
    }

    public async Task<string> Register(UserLoginModel userInfo)
    {
        #region Find User

        User? existingUser = await _fsContext.Users.Where(x => x.Username.ToLower() == userInfo.Username.ToLower())
            .FirstOrDefaultAsync();

        if (existingUser is not null) throw new UserException("Username Taken");

        #endregion
        
        #region Check For Bad Chars

        char[] badChars = new char[]{ '<','>','.','?',';',':','/','!','@','#','$','%','^','&','*','(',')' };
        foreach (var c in badChars)
        {
            if (userInfo.Username != null && userInfo.Username.Contains(c)) throw new UserException("Bad Character: " + c);
        }

        #endregion
        
        #region Add User

        var user = new User()
        {
            Username = userInfo.Username,
            Passhash = BCrypt.Net.BCrypt.HashPassword(userInfo.Password)
        };

        await _fsContext.Users.AddAsync(user);
        await _fsContext.SaveChangesAsync();

        User dbUser = (await _fsContext.Users.FirstOrDefaultAsync(x => x.Username == user.Username))!;
        dbUser.Passhash = null!;

        return _jwtService.EncodeToken(dbUser);

        #endregion
    }
    public async Task<string> Login(UserLoginModel userInfo)
    {
        #region Find User

        User? existingUser = await _fsContext.Users.Where(x => x.Username.ToLower() == userInfo.Username.ToLower())
            .FirstOrDefaultAsync();

        if (existingUser is null) throw new UserException("User not found");

        #endregion

        #region Check Password

        if (!BCrypt.Net.BCrypt.Verify(userInfo.Password, existingUser.Passhash)) throw new UserException("Invalid Password");

        #endregion

        #region Return Jwt

        return _jwtService.EncodeToken(existingUser);

        #endregion
    }

    

    public async Task<User> GetMe(HttpContext ctx)
    {
        try
        {
            var user = await _jwtService.DecodeToken(ctx.Request.Cookies["fsAuth"]!);
            user.Passhash = null!;
            return user;
        }
        catch (UserException e)
        {
            throw new UserException(e.Message);
        }
        
    }
}