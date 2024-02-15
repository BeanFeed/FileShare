using FileshareBackend.Exceptions;
using FileshareBackend.Models;
using FileshareBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;

namespace FileshareBackend.Controllers;

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class FileSystemController : ControllerBase
{
    private readonly IFileSystemService _fileSystemService;
    private readonly IConfiguration _config;
    public FileSystemController(IFileSystemService fsS, IConfiguration config)
    {
        _fileSystemService = fsS;
        _config = config;
    }
    [HttpGet]
    public IActionResult GetFromDirectory([FromQuery]string[] pathArr)
    {
        
        try
        {
            FSEntryModel items = _fileSystemService.GetFromDirectory(pathArr);
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
    public IActionResult MoveItem(MoveItemModel data)
    {
        if (data.itemName != null)
        {
            try
            {
                _fileSystemService.MoveFile(data.oldPath, data.newPath, data.itemName, data.overwrite);
            }
            catch (FileSystemException e)
            {
                ResponseModel<string> res = new ResponseModel<string>(false, e.Message);
                return BadRequest(res);
            }

            return Ok(new ResponseModel<string>(true, "File moved"));
        }
        else
        {
            try
            {
                _fileSystemService.MoveDirectory(data.oldPath, data.newPath);
            }
            catch (FileSystemException e)
            {
                ResponseModel<string> res = new ResponseModel<string>(false, e.Message);
                return BadRequest(res);
            }
            return Ok(new ResponseModel<string>(true, "Directory moved"));
        }
    }

    [HttpPost]
    public IActionResult RenameItem(RenameItemModel data)
    {
        try
        {
            _fileSystemService.RenameItem(data.ItemPath, data.NewName);
        }
        catch (FileSystemException e)
        {
            ResponseModel<string> res = new ResponseModel<string>(false, e.Message);
            return BadRequest(res);
        }
        return Ok(new ResponseModel<string>(true, "Item renamed"));
    }
    
    

    [HttpDelete]
    public IActionResult DeleteItem([FromBody] string[] path)
    {
        try
        {
            _fileSystemService.DeleteItem(path);
        }
        catch (FileSystemException e)
        {
            ResponseModel<string> res = new ResponseModel<string>(false, e.Message);
            return BadRequest(res);
        }
        
        return Ok(new ResponseModel<string>(true, "Item deleted"));
    }

    [HttpPost]
    public IActionResult UploadFile([FromForm] FileUploadModel uploadModel)
    {
        try
        {
            _fileSystemService.UploadFile(uploadModel.Path, uploadModel.File);
        }
        catch (FileSystemException e)
        {
            return BadRequest(new ResponseModel<string>(false, e.Message));
        }
        return Ok(new ResponseModel<string>(true, "File uploaded"));
    }

    [HttpGet]
    public IActionResult DownloadFile([FromQuery] string[] pathArr, [FromQuery] bool? raw)
    {
        for (int i = 0; i < pathArr.Length; i++)
        {
            pathArr[i] = Utils.CleanString(pathArr[i]);
        }
        string path = "";
        try
        {
            
            path = Path.Join(_config["DirectoryRootPath"], string.Join('/', pathArr));
        }
        catch
        {
            return BadRequest(new ResponseModel<string>(false, "Failed to locate directory"));
        }
        if (!System.IO.File.Exists(path)) return BadRequest(new ResponseModel<string>(false, "File doesn't exists"));
        var stream = System.IO.File.OpenRead(path);
        var type = MimeTypeMap.GetMimeType(pathArr[^1]);
        if (raw is true)
        {
            return File(stream, type);
        }
        else
        {
            return File(stream, type, pathArr[^1]);
        }
    }
    
}