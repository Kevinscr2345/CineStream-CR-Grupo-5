using System.Security.Claims;
using CineStreamCR.BLL.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CineStreamCR.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected Guid GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(value, out var userId))
            throw new UnauthorizedAccessException("Sesión inválida.");
        return userId;
    }

    protected IActionResult FromResponse<T>(Respuesta<T> response)
    {
        return StatusCode(response.Codigo, response);
    }
}

