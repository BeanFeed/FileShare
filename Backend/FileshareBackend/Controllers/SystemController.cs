using FileshareBackend.Models;
using FileshareBackend.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FileshareBackend.Controllers;

[ApiController]
[Route("v1/[controller]/[action]")]
public class SystemController : ControllerBase
{
    private readonly FrontendSettings _frontendSettings;

    public SystemController(IOptions<FrontendSettings> frontendSettings)
    {
        _frontendSettings = frontendSettings.Value;
    }

    [HttpGet]
    public IActionResult GetFrontendSettings()
    {
        return Ok(new ResponseModel<FrontendSettings>(true, _frontendSettings));
    }
}