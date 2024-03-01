using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DAL.Context;
using DAL.Entities;
using FileshareBackend.Exceptions;
using FileshareBackend.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace FileshareBackend.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    private readonly FileShareContext _fsContext;
    
    public JwtService(IConfiguration config, FileShareContext fsContext)
    {
        _config = config;
        _fsContext = fsContext;
    }
    
    public string EncodeToken(User user)
    {
        Console.WriteLine(_config["Jwt:Key"]);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("ID", user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
            );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public async Task<User> DecodeToken(string eToken)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var handler = new JwtSecurityTokenHandler();
        /*
        try
        {

        }
        catch (Exception e)
        {

        }
        */

        TokenValidationResult result = await handler.ValidateTokenAsync(eToken,new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true
        });
        if (!result.IsValid) throw new UserException("Session Expired");
        JwtSecurityToken token = handler.ReadJwtToken(eToken);
        var usr = await SearchUser(long.Parse(token.Claims.Where(claim => claim.Type == "ID").ToArray()[0].Value));
        if (usr == null) throw new UserException("Invalid Token");
        return usr;
    }
    
    private async Task<User?> SearchUser(long id)
    {
        User? user = await _fsContext.Users.FindAsync(id);
        
        return user;
    }

}