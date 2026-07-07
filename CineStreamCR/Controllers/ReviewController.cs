using CineStreamCR.BLL.Dtos;
using CineStreamCR.BLL.Dtos.Review;
using CineStreamCR.BLL.Services;
using CineStreamCR.BLL.Services.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineStreamCR.Controllers;

[Authorize]
[IgnoreAntiforgeryToken]
[Route("api/movies")]
public sealed class ReviewController(IReviewService reviewService) : ApiControllerBase
{
    [HttpPut("{movieId:int}/review")]
    public async Task<IActionResult> Save(int movieId, [FromBody] ReviewRequest request, CancellationToken ct)
    {
        return FromResponse(await reviewService.UpsertAsync(GetUserId(), movieId, request, ct));
    }
}