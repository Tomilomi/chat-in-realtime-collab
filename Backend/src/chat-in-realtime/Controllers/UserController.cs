using Application.Common;
using Application.Interfaces;
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

        [HttpPut]
        public async Task<IActionResult> Update()
        {
            throw new NotImplementedException();
        }
    }
}