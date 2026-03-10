using System.Security.Claims;
using Application.Common;
using Application.Common.Users;
using Application.Interfaces.User;
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
        [Authorize(Roles = "Admin")]
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
                username = result.Value.Username,
                role = result.Value.Role.ToString()
            });
        }

        [HttpPost("{id}/ban")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ban(Guid id)
        {
            var result = await _userService.BanAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/unban")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Unban(Guid id)
        {
            var result = await _userService.UnbanAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole(Guid id, [FromBody] ChangeRoleRequestDTO request)
        {
            var result = await _userService.ChangeRoleAsync(id, request.Role);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("profiles")]
        [Authorize]
        public async Task<IActionResult> GetAllProfiles()
        {
            var result = await _userService.GetAllProfilesAsync();
            return result.Match(
                profiles => Ok(profiles),
                errors => Problem(errors));
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMe([FromBody] UserUpdateRequestDTO request)
        {
            string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var result = await _userService.UpdateAsync(Guid.Parse(userIdString), request);
            return result.Match(
                updated => NoContent(),
                errors => Problem(errors));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateRequestDTO request)
        {
            var result = await _userService.UpdateAsync(id, request);
            return result.Match(
                updated => NoContent(),
                errors => Problem(errors));
        }
    }
}