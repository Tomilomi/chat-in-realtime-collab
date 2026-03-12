using Application.Common.Auth;
using Application.Interfaces;
using Application.Interfaces.Users;
using Microsoft.AspNetCore.Mvc;

namespace chat_in_realtime.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ApiController
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public AuthController(IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDTO request)
    {
        await _userService.RegisterAsync(request.Username, request.Password);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDTO request)
    {
        var resultLogin = await _userService.LoginAsync(request.Username, request.Password);
        if (resultLogin.IsError) return Unauthorized();
        var user = resultLogin.Value;

        var tokenRequest = new GenerateTokenRequestDTO(
            UserId: user.Id,
            Username: user.Username,
            Role: user.Role);

        var resultToken = _tokenService.GenerateToken(tokenRequest);

        return resultToken.Match(
            token => Ok(new{ token }),
            errors => Problem(errors));
    }
}