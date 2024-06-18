using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DAL.Context;
using DAL.Entities;
using FileshareBackend.Exceptions;
using FileshareBackend.Models.Settings;
using FileshareBackend.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FileshareBackend.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly FileShareContext _fsContext;
    
    public JwtService(IOptions<JwtSettings> jwtSettings, FileShareContext fsContext)
    {
        _jwtSettings = jwtSettings.Value;
        _fsContext = fsContext;
    }
    
    public string[] EncodeToken(User user)
    {
        Console.WriteLine(_jwtSettings.Key);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var rsecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.RKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var rCredentials = new SigningCredentials(rsecurityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("ID", user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
            );

        var rToken = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: rCredentials
            );
        string[] tokens = { new JwtSecurityTokenHandler().WriteToken(token), new JwtSecurityTokenHandler().WriteToken(rToken) };
        return tokens;
    }
    
    public async Task<User> DecodeToken(string eToken, bool isRefresh)
    {
        if (!isRefresh)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
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
            if (!result.IsValid) throw new UserException("Access Token Expired");
            JwtSecurityToken token = handler.ReadJwtToken(eToken);
            var usr = await SearchUser(long.Parse(token.Claims.Where(claim => claim.Type == "ID").ToArray()[0].Value));
            if (usr == null) throw new UserException("Invalid Token");
            return usr;
        }
        else
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.RKey));
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
        
    }
    
    private async Task<User?> SearchUser(long id)
    {
        User? user = await _fsContext.Users.FindAsync(id);
        
        return user;
    }

}