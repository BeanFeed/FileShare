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
    public IActionResult GetFromDirectory(string path)
    {
        
        try
        {
            FSEntryModel items = _fileSystemService.GetFromDirectory(path);
            ResponseModel res = new ResponseModel(true, items);
            return Ok(res);
        }
        catch (FileSystemException e)
        {
            ResponseModel res = new ResponseModel(false, e.Message);
            return NotFound(res);
        }
        
        
    }
}