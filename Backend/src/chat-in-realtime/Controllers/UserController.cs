using System.Security.Claims;
using Application.Common;
using Application.Common.Users;
using Application.Interfaces;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace chat_in_realtime.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return result.Match(
                users => Ok(users),
                errors => Problem(errors));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var result = await _userService.GetByIdAsync(Guid.Parse(userIdString));
            if (result.IsError) return Unauthorized();

            return Ok(new
            {
                id = result.Value.Id,
                username = result.Value.Username
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequestDTO request)
        {
            var result = await _userService.UpdateAsync(id, request);
            return result.Match(
                updated => NoContent(),
                errors => Problem(errors));
        }
    }
}