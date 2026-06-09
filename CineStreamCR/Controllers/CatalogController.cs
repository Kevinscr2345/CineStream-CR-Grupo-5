using CineStreamCR.BLL.Dtos;
using CineStreamCR.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineStreamCR.Controllers;

[Authorize]
[Route("api")]
public sealed class CatalogController(IMovieService movieService) : ApiControllerBase
{
    [HttpGet("genres")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Genres(CancellationToken ct) => FromResponse(await movieService.GetGenresAsync(ct));

    [HttpGet("years")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Years(CancellationToken ct) => FromResponse(await movieService.GetYearsAsync(ct));

    [HttpGet("movies/featured")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Featured(CancellationToken ct) => FromResponse(await movieService.GetFeaturedAsync(GetUserId(), ct));

    [HttpGet("movies")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Movies(
        string? search, int? genreId, int? year, string? sort, string? direction, int? page, int? pageSize,
        CancellationToken ct)
    {
        var query = new MovieQuery(search, genreId, year, sort ?? "title", direction ?? "asc", page ?? 1, pageSize ?? 8);
        return FromResponse(await movieService.GetCatalogAsync(GetUserId(), query, ct));
    }

    [HttpGet("movies/{id:int}")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Movie(int id, CancellationToken ct) => FromResponse(await movieService.GetByIdAsync(GetUserId(), id, ct));
}

