using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using CineStreamCR.BLL.Services.Media;

namespace CineStreamCR.Controllers;

[ApiController]
[Route("api/media")]
public sealed class MediaController(IMediaService mediaService) : ControllerBase
{
    [HttpGet("wiki-thumbnail")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> WikiThumbnail(
        [FromQuery] string? title,
        [FromQuery] string? fallback,
        CancellationToken ct)
    {
        var imageUrl = await mediaService.GetWikipediaThumbnailAsync(title, fallback, ct);

        return Redirect(imageUrl);
    }
}

