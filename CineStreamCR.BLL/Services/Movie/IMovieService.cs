using CineStreamCR.BLL.Dtos;

namespace CineStreamCR.BLL.Services;

public interface IMovieService
{
    Task<Respuesta<IReadOnlyList<GenreDto>>> GetGenresAsync(CancellationToken ct = default);
    Task<Respuesta<IReadOnlyList<int>>> GetYearsAsync(CancellationToken ct = default);
    Task<Respuesta<PagedResult<MovieCardDto>>> GetCatalogAsync(Guid userId, MovieQuery query, CancellationToken ct = default);
    Task<Respuesta<MovieDetailDto>> GetByIdAsync(Guid userId, int movieId, CancellationToken ct = default);
    Task<Respuesta<MovieCardDto>> GetFeaturedAsync(Guid userId, CancellationToken ct = default);
}
