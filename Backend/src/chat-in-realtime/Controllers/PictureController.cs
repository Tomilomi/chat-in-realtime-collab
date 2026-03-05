using Application.Common;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace chat_in_realtime.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PictureController : ApiController
{
    private readonly IPictureService _pictureService;
    
    public PictureController(IPictureService pictureService)
    {
        _pictureService = pictureService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var pictures = await _pictureService.GetAllAsync();
        var result = pictures.Select(p => new PictureDTO(p.Id, p.Url));
        return Ok(result);
    }
}