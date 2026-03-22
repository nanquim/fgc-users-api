using Microsoft.AspNetCore.Mvc;
using FGC.Users.Application.DTOs;
using FGC.Users.Application.Services;

namespace FGC.Users.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var token = await _authService.LoginAsync(request);
        return Ok(new { token });
    }
}
