using CineStreamCR.BLL.Dtos;

namespace CineStreamCR.BLL.Services;

public interface IWatchListService
{
    Task<Respuesta<IReadOnlyList<WatchListDto>>> GetAllAsync(Guid userId, CancellationToken ct = default);
    Task<Respuesta<WatchListDetailDto>> GetByIdAsync(Guid userId, int watchListId, CancellationToken ct = default);
    Task<Respuesta<WatchListDto>> CreateAsync(Guid userId, CreateWatchListRequest request, CancellationToken ct = default);
    Task<Respuesta<object>> UpdateAsync(Guid userId, int watchListId, UpdateWatchListRequest request, CancellationToken ct = default);
    Task<Respuesta<object>> DeleteAsync(Guid userId, int watchListId, CancellationToken ct = default);
    Task<Respuesta<object>> AddMovieAsync(Guid userId, int watchListId, int movieId, CancellationToken ct = default);
    Task<Respuesta<object>> RemoveMovieAsync(Guid userId, int watchListId, int movieId, CancellationToken ct = default);
}
