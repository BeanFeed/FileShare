using DAL.Context;
using DAL.Entities;
using FileshareBackend.Exceptions;
using FileshareBackend.Models;
using FileshareBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FileshareBackend.Services;

public class UserService : IUserService
{
    private readonly FileShareContext _fsContext;
    private readonly IJwtService _jwtService;
    
    public UserService(FileShareContext context, IJwtService jwtService)
    {
        _fsContext = context;
        _jwtService = jwtService;
    }

    public async Task<string[]> Register(UserLoginModel userInfo)
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
    public async Task<string[]> Login(UserLoginModel userInfo)
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

    public async Task ChangePassword(ChangePasswordModel passwordModel)
    {
        #region Find User

        User? existingUser = await _fsContext.Users.Where(x => x.Username.ToLower() == passwordModel.Username.ToLower())
            .FirstOrDefaultAsync();

        if (existingUser is null) throw new UserException("User not found");

        #endregion
        
        #region Check Password

        if (!BCrypt.Net.BCrypt.Verify(passwordModel.OldPassword, existingUser.Passhash)) throw new UserException("Invalid Password");

        #endregion

        #region Change Password

        if (passwordModel.NewPassword != passwordModel.Retype) throw new UserException("Passwords do not match");

        existingUser.Passhash = BCrypt.Net.BCrypt.HashPassword(passwordModel.NewPassword);

        _fsContext.Users.Update(existingUser);

        await _fsContext.SaveChangesAsync();

        #endregion
    }

    public async Task<string[]> Refresh(string rToken)
    {
        try
        {
            var user = await _jwtService.DecodeToken(rToken, true);
            return _jwtService.EncodeToken(user);

        }
        catch (UserException e)
        {
            throw new UserException(e.Message);
        }
    }

    public async Task<User> GetMe(string ctx)
    {
        try
        {
            var user = await _jwtService.DecodeToken(ctx, false);
            user.Passhash = null!;
            return user;
        }
        catch (UserException e)
        {
            throw new UserException(e.Message);
        }
        
    }
}