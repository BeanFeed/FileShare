using DAL.Entities;
using FileshareBackend.Exceptions;
using FileshareBackend.Models;
using FileshareBackend.Models.Settings;
using FileshareBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeTypes;

namespace FileshareBackend.Controllers;

[ApiController]
[Route("v1/[controller]/[action]")]
public class FileSystemController : ControllerBase
{
    private readonly IFileSystemService _fileSystemService;
    private readonly IJwtService _jwtService;
    private readonly GeneralSettings _generalSettings;
    public FileSystemController(IFileSystemService fsS, IJwtService jwtService, IOptions<GeneralSettings> generalSettings)
    {
        _fileSystemService = fsS;
        _jwtService = jwtService;
        _generalSettings = generalSettings.Value;
    }
    [HttpGet]
    public async Task<IActionResult> GetFromDirectory([FromQuery]string[] pathArr)
    {
        
        try
        {
            User? user = null;
            try
            {
                user = Request.Cookies["fsAuth"] != null
                    ? await _jwtService.DecodeToken(Request.Cookies["fsAuth"]!, false)
                    : null;
            }
            catch (Exception e)
            {
            }
            FSEntryModel items = _fileSystemService.GetFromDirectory(pathArr, user);
            ResponseModel<FSEntryModel> res = new ResponseModel<FSEntryModel>(true, items);
            return Ok(res);
        }
        catch (Exception e)
        {
            ResponseModel<string> res = new ResponseModel<string>(false, e.Message);
            return NotFound(res);
        }
        
    }

    [HttpGet]
    public async Task<IActionResult> GetItemProperties([FromQuery]ItemPropertiesReqModel item)
    {
        try
        {
            User? user = Request.Cookies["fsAuth"] != null
                ? await _jwtService.DecodeToken(Request.Cookies["fsAuth"], false)
                : null;
            if (user is null) return Unauthorized();
            var properties = _fileSystemService.GetItemProperties(item.pathArr, item.isDirectory, user);
            return Ok(new ResponseModel<ItemPropertiesResModel>(true, properties));

        }
        catch (FileSystemException e)
        {
            ResponseModel<string> res = new ResponseModel<string>(false, e.Message);
            return BadRequest(res);
        }
    }

    [HttpPost]
    public async Task<IActionResult> MoveItem(MoveItemModel data)
    {
        if (data.itemName != null)
        {
            try
            {
                User? user = Request.Cookies["fsAuth"] != null
                    ? await _jwtService.DecodeToken(Request.Cookies["fsAuth"], false)
                    : null;
                if (user is null) return Unauthorized();
                _fileSystemService.MoveFile(data.oldPath, data.newPath, data.itemName, user, data.overwrite);
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
                User? user = Request.Cookies["fsAuth"] != null
                    ? await _jwtService.DecodeToken(Request.Cookies["fsAuth"], false)
                    : null;
                if (user is null) return Unauthorized();
                _fileSystemService.MoveDirectory(data.oldPath, data.newPath, user);
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
    public async Task<IActionResult> RenameItem(RenameItemModel data)
    {
        try
        {
            User? user = Request.Cookies["fsAuth"] != null
                ? await _jwtService.DecodeToken(Request.Cookies["fsAuth"], false)
                : null;
            if (user is null) return Unauthorized();
            _fileSystemService.RenameItem(data.ItemPath, data.NewName, user);
        }
        catch (FileSystemException e)
        {
            ResponseModel<string> res = new ResponseModel<string>(false, e.Message);
            return BadRequest(res);
        }
        return Ok(new ResponseModel<string>(true, "Item renamed"));
    }
    
    

    [HttpDelete]
    public async Task<IActionResult> DeleteItem([FromBody] string[] path)
    {
        try
        {
            User? user = Request.Cookies["fsAuth"] != null
                ? await _jwtService.DecodeToken(Request.Cookies["fsAuth"], false)
                : null;
            if (user is null) return Unauthorized();
            _fileSystemService.DeleteItem(path, user);
        }
        catch (FileSystemException e)
        {
            ResponseModel<string> res = new ResponseModel<string>(false, e.Message);
            return BadRequest(res);
        }
        
        return Ok(new ResponseModel<string>(true, "Item deleted"));
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile([FromForm] FileUploadModel uploadModel)
    {
        try
        {
            User? user = Request.Cookies["fsAuth"] != null
                ? await _jwtService.DecodeToken(Request.Cookies["fsAuth"], false)
                : null;
            if (user is null) return Unauthorized();
            _fileSystemService.UploadFile(uploadModel.Path, uploadModel.File, user);
        }
        catch (FileSystemException e)
        {
            return BadRequest(new ResponseModel<string>(false, e.Message));
        }
        return Ok(new ResponseModel<string>(true, "File uploaded"));
    }

    [HttpGet]
    public async Task<IActionResult> DownloadFile([FromQuery] string[] pathArr, [FromQuery] bool? raw)
    {
        for (int i = 0; i < pathArr.Length; i++)
        {
            pathArr[i] = Utils.CleanString(pathArr[i]);
        }
        string path = "";
        try
        {
            
            path = Path.Join(_generalSettings.DirectoryRootPath, string.Join('/', pathArr));
        }
        catch
        {
            return BadRequest(new ResponseModel<string>(false, "Failed to locate directory"));
        }
        User? user = null;
        try
        {
            user = Request.Cookies["fsAuth"] != null
                ? await _jwtService.DecodeToken(Request.Cookies["fsAuth"]!, false)
                : null;
        }
        catch (Exception e)
        {
        }

        if (!_fileSystemService.CanDownload(pathArr, user)) return Unauthorized();
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

    [HttpPost]
    public IActionResult ChangeItemVisibility(ItemVisibilityModel item)
    {
        try
        {
            _fileSystemService.ChangeItemVisibility(item.pathArr, item.visibility);
            return Ok(new ResponseModel<string>(true, "Visibility Updated"));
        }
        catch (FileSystemException e)
        {
            return BadRequest(new ResponseModel<string>(false, e.Message));
        }
    }

    [HttpPost]
    public IActionResult ChangePublicUploading(PublicUploadingModel model)
    {
        try
        {
            _fileSystemService.ChangePublicUploading(model.pathArr, model.anyoneCanUpload);
            return Ok(new ResponseModel<string>(true, "Public Uploading Updated"));
        }
        catch (FileSystemException e)
        {
            return BadRequest(new ResponseModel<string>(true, e.Message));
        }
    }
    
}