using DAL.Entities;
using FileshareBackend.Exceptions;
using FileshareBackend.Models;
using FileshareBackend.Models.Settings;
using FileshareBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FileshareBackend.Controllers;

[ApiController]
[Route("v1/[controller]/[action]")]
public class UserController : ControllerBase
{
    //private readonly IConfiguration _config;
    private readonly JwtSettings _jwtSettings;
    private readonly IUserService _userService;
    
    public UserController(IConfiguration config, IUserService userService, IOptions<JwtSettings> jwtSettings)
    {
        //_config = config;
        _jwtSettings = jwtSettings.Value;
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
                Secure = Convert.ToBoolean(_jwtSettings.Secure),
                SameSite = Utils.GetSSM(_jwtSettings.SSM)
                
            };
            /*
            var rjwtOpt = new CookieOptions()
            {
                Domain = Request.Host.Host,
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Secure = Convert.ToBoolean(_jwtSettings.Secure),
                SameSite = Utils.GetSSM(_jwtSettings.SSM)
                
            };
            */
            HttpContext.Response.Cookies.Append("fsAuth", token[0], jwtOpt);
            //HttpContext.Response.Cookies.Append("rfsAuth", token[1], rjwtOpt);
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
                Secure = Convert.ToBoolean(_jwtSettings.Secure),
                SameSite = Utils.GetSSM(_jwtSettings.SSM)
                
            };
            /*
            var rjwtOpt = new CookieOptions()
            {
                Domain = Request.Host.Host,
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Secure = Convert.ToBoolean(_jwtSettings.Secure),
                SameSite = Utils.GetSSM(_jwtSettings.SSM)
                
            };
            */
            Response.Cookies.Append("fsAuth", token[0], jwtOpt);
            //Response.Cookies.Append("rfsAuth", token[1], rjwtOpt);
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
        //Response.Cookies.Delete("fsAuth",new CookieOptions(){Domain = Request.Host.Host, HttpOnly = true, Secure = Convert.ToBoolean(_config["Jwt:Secure"]), SameSite = Utils.GetSSM(_config)});
        Response.Cookies.Append("fsAuth","",new CookieOptions(){Domain = Request.Host.Host, Secure = Convert.ToBoolean(_jwtSettings.Secure), SameSite = Utils.GetSSM(_jwtSettings.SSM), Expires = DateTime.UtcNow.AddDays(-1)});
        //Response.Cookies.Append("rfsAuth","",new CookieOptions(){Domain = Request.Host.Host, Secure = Convert.ToBoolean(_jwtSettings.Secure), SameSite = Utils.GetSSM(_jwtSettings.SSM), Expires = DateTime.UtcNow.AddDays(-1)});
        
        return Ok(new ResponseModel<string>(true, "User signed out"));
    }
/*
    [HttpGet]
    public async Task<IActionResult> Refresh(string? action, string? controller)
    {
        if (Request.Cookies["rfsAuth"] == null) return BadRequest(new ResponseModel<string>(false, "Session Expired"));
        string[] tokens = new string[2];
        try
        {
            tokens = await _userService.Refresh(Request.Cookies["rfsAuth"]!);
        }
        catch (UserException e)
        {
            return BadRequest(new ResponseModel<string>(false, e.Message));
        }
        
        var jwtOpt = new CookieOptions()
        {
            Domain = Request.Host.Host,
            HttpOnly = true,
            Expires = DateTimeOffset.UtcNow.AddMinutes(15),
            Secure = Convert.ToBoolean(_jwtSettings.Secure),
            SameSite = Utils.GetSSM(_jwtSettings.SSM)
                
        };
            
        var rjwtOpt = new CookieOptions()
        {
            Domain = Request.Host.Host,
            HttpOnly = true,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Secure = Convert.ToBoolean(_jwtSettings.Secure),
            SameSite = Utils.GetSSM(_jwtSettings.SSM)
                
        };
        
        Response.Cookies.Append("fsAuth", tokens[0],jwtOpt);
        Response.Cookies.Append("rfsAuth",tokens[1],rjwtOpt);
        if(action == null || controller == null) return Ok(new ResponseModel<string>(true, "Refeshed token"));
        return RedirectToAction(action, controller);
    }
    */
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
            /*
            if (e.Message == "Access Token Expired")
            {
                await this.Refresh(null, null);
            }
                
            return RedirectToAction("Me", "User");
            */
            return BadRequest(new ResponseModel<string>(false, e.Message));
        }
    }

    
}