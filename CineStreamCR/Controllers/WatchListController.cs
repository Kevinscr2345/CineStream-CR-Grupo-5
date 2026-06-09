using CineStreamCR.BLL.Dtos;
using CineStreamCR.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineStreamCR.Controllers;

[Authorize]
[Route("api/watchlists")]
public sealed class WatchListController(IWatchListService watchListService) : ApiControllerBase
{
    [HttpGet]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> GetAll(CancellationToken ct) => FromResponse(await watchListService.GetAllAsync(GetUserId(), ct));

    [HttpGet("{id:int}")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Get(int id, CancellationToken ct) => FromResponse(await watchListService.GetByIdAsync(GetUserId(), id, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWatchListRequest request, CancellationToken ct) =>
        FromResponse(await watchListService.CreateAsync(GetUserId(), request, ct));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWatchListRequest request, CancellationToken ct) =>
        FromResponse(await watchListService.UpdateAsync(GetUserId(), id, request, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
        FromResponse(await watchListService.DeleteAsync(GetUserId(), id, ct));

    [HttpPost("{id:int}/movies/{movieId:int}")]
    public async Task<IActionResult> AddMovie(int id, int movieId, CancellationToken ct) =>
        FromResponse(await watchListService.AddMovieAsync(GetUserId(), id, movieId, ct));

    [HttpDelete("{id:int}/movies/{movieId:int}")]
    public async Task<IActionResult> RemoveMovie(int id, int movieId, CancellationToken ct) =>
        FromResponse(await watchListService.RemoveMovieAsync(GetUserId(), id, movieId, ct));
}

