using Application.Interfaces;

namespace chat_in_realtime.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessageController : ApiController
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin, Moderator")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _messageService.DeleteMessageAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
    
    
}