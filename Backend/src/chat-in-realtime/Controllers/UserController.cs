using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common;

namespace chat_in_realtime.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public UserController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDTO request)
    {
        Console.WriteLine($"Registrando: {request.Username}");
        await _userService.RegisterAsync(request.Username, request.Password);
        Console.WriteLine($"Registro completado");
        return Ok();
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDTO request)
    {
        Console.WriteLine($"Login intento: {request.Username} / {request.Password}");
        var user = await _userService.LoginAsync(request.Username, request.Password);
        Console.WriteLine($"Usuario encontrado: {user?.Username ?? "null"}");
        if (user is null) return Unauthorized();

        var token = GenerateJwtToken(user.Id, user.Username);
        return Ok(new { token });
    }
    
    private string GenerateJwtToken(Guid userId, string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    
}