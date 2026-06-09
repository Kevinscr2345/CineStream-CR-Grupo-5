using CineStreamCR.BLL.Dtos;
using CineStreamCR.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineStreamCR.Controllers;

[Authorize]
[Route("api/movies")]
public sealed class ReviewController(IReviewService reviewService) : ApiControllerBase
{
    [HttpPut("{movieId:int}/review")]
    public async Task<IActionResult> Save(int movieId, [FromBody] ReviewRequest request, CancellationToken ct) =>
        FromResponse(await reviewService.UpsertAsync(GetUserId(), movieId, request, ct));
}

