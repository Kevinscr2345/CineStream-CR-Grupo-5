using System.Security.Claims;
using CineStreamCR.BLL.Dtos;
using CineStreamCR.BLL.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineStreamCR.Controllers;

[Route("api/auth")]
public sealed class AuthController(IUserService userService) : ApiControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var response = await userService.ValidateCredentialsAsync(request.Identifier, request.Password, ct);
        if (!response.EsCorrecto || response.Dato is null) return FromResponse(response);

        var user = response.Dato;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        return FromResponse(response);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return FromResponse(Respuesta<object>.Correcta(null, "Sesión cerrada correctamente."));
    }

    [Authorize]
    [HttpGet("me")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Me(CancellationToken ct) => FromResponse(await userService.GetByIdAsync(GetUserId(), ct));
}

