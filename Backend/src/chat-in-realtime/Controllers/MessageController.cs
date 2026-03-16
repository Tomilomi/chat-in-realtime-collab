using Application.Interfaces;
using Application.Interfaces.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace chat_in_realtime.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessageController : ApiController
{
    private readonly IMessageService _messageService;
    private readonly IChatNotificationService _chatNotificationService;

    public MessageController(IMessageService messageService, IChatNotificationService chatNotificationService)
    {
        _messageService = messageService;
        _chatNotificationService = chatNotificationService;
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _messageService.DeleteMessageAsync(id);
        if (!result) return NotFound();
        await _chatNotificationService.NotifyMessageDeletedAsync(id);
        return NoContent();
    }
}