using FileshareBackend.Exceptions;
using FileshareBackend.Models;
using FileshareBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileshareBackend.Controllers;

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class FileSystemController : ControllerBase
{
    private readonly IFileSystemService _fileSystemService;
    public FileSystemController(IFileSystemService fsS)
    {
        _fileSystemService = fsS;
    }
    [HttpGet]
    public IActionResult GetFromDirectory([FromQuery]string[] path)
    {
        
        try
        {
            FSEntryModel items = _fileSystemService.GetFromDirectory(path);
            ResponseModel<FSEntryModel> res = new ResponseModel<FSEntryModel>(true, items);
            return Ok(res);
        }
        catch (FileSystemException e)
        {
            ResponseModel<string> res = new ResponseModel<string>(false, e.Message);
            return NotFound(res);
        }
        
    }

    [HttpPost]
    public IActionResult MoveItem(string[] path)
    {
        
    }
}