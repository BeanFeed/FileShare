using DAL.Entities;
using FileshareBackend.Exceptions;
using FileshareBackend.Models;
using FileshareBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileshareBackend.Controllers;

[ApiController]
[Route("v1/[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IUserService _userService;
    
    public UserController(IConfiguration config, IUserService userService)
    {
        _config = config;
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody]UserLoginModel userInfo)
    {
        
        
        try
        {
            var token = await _userService.Register(userInfo);
            var jwtOpt = new CookieOptions()
            {
                Domain = Request.Host.Host,
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Secure = Convert.ToBoolean(_config["Jwt:Secure"]),
                SameSite = Utils.GetSSM(_config)
                
            };
            HttpContext.Response.Cookies.Append("fsAuth", token, jwtOpt);
            return Ok(new ResponseModel<string>(true, "Register Successful"));
        }
        catch (UserException e)
        {
            return BadRequest(new ResponseModel<string>(false, e.Message));
        }
    
    }
    
    [HttpPost]
    public async Task<IActionResult> Login([FromBody]UserLoginModel userInfo)
    {
        
        
        try
        {
            var token = await _userService.Login(userInfo);
            var jwtOpt = new CookieOptions()
            {
                Domain = Request.Host.Host,
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Secure = Convert.ToBoolean(_config["Jwt:Secure"]),
                SameSite = Utils.GetSSM(_config)
                
            };
            Response.Cookies.Append("fsAuth", token, jwtOpt);
            return Ok(new ResponseModel<string>(true, "Login Successful"));
        }
        catch (UserException e)
        {
            return BadRequest(new ResponseModel<string>(false, e.Message));
        }
    
    }
    
    [HttpGet]
    public IActionResult Signout()
    {
        if (Request.Cookies["fsAuth"] == null) return BadRequest(new ResponseModel<string>(false, "Session Expired"));
        //Response.Cookies.Delete("token",new CookieOptions(){Secure = Convert.ToBoolean(_config["Jwt:Secure"]), SameSite = SameSiteMode.None});
        Response.Cookies.Append("fsAuth","",new CookieOptions(){Domain = Request.Host.Host, Secure = Convert.ToBoolean(_config["Jwt:Secure"]), SameSite = Utils.GetSSM(_config), Expires = DateTime.UtcNow.AddDays(-1)});
        return Ok(new ResponseModel<string>(true, "User signed out"));
    }

    [HttpGet]
    public async Task<IActionResult> Me()
    {
        try
        {
            var user = await _userService.GetMe(Request.Cookies["fsAuth"]!);
            return Ok(new ResponseModel<User>(true, user));
        }
        catch(UserException e)
        {
            return BadRequest(new ResponseModel<string>(false, e.Message));
        }
    }

    
}