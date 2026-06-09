using CineStreamCR.BLL.Dtos;
using CineStreamCR.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineStreamCR.Controllers;

[Authorize]
[Route("api/playback")]
public sealed class PlaybackController(IPlaybackService playbackService) : ApiControllerBase
{
    [HttpGet("{movieId:int}")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Get(int movieId, CancellationToken ct) =>
        FromResponse(await playbackService.GetAsync(GetUserId(), movieId, ct));

    [HttpPut]
    public async Task<IActionResult> Save([FromBody] PlaybackRequest request, CancellationToken ct) =>
        FromResponse(await playbackService.SaveAsync(GetUserId(), request, ct));
}

