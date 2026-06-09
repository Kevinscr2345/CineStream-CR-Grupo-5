using CineStreamCR.BLL.Dtos;

namespace CineStreamCR.BLL.Services;

public interface IUserService
{
    Task<Respuesta<UserDto>> ValidateCredentialsAsync(string identifier, string password, CancellationToken ct = default);
    Task<Respuesta<UserDto>> GetByIdAsync(Guid userId, CancellationToken ct = default);
}

public interface IMovieService
{
    Task<Respuesta<IReadOnlyList<GenreDto>>> GetGenresAsync(CancellationToken ct = default);
    Task<Respuesta<IReadOnlyList<int>>> GetYearsAsync(CancellationToken ct = default);
    Task<Respuesta<PagedResult<MovieCardDto>>> GetCatalogAsync(Guid userId, MovieQuery query, CancellationToken ct = default);
    Task<Respuesta<MovieDetailDto>> GetByIdAsync(Guid userId, int movieId, CancellationToken ct = default);
    Task<Respuesta<MovieCardDto>> GetFeaturedAsync(Guid userId, CancellationToken ct = default);
}

public interface IPersonService
{
    Task<Respuesta<PersonProfileDto>> GetByIdAsync(int personId, CancellationToken ct = default);
}

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

public interface IReviewService
{
    Task<Respuesta<ReviewDto>> UpsertAsync(Guid userId, int movieId, ReviewRequest request, CancellationToken ct = default);
}

public interface IPlaybackService
{
    Task<Respuesta<PlaybackDto?>> GetAsync(Guid userId, int movieId, CancellationToken ct = default);
    Task<Respuesta<PlaybackDto>> SaveAsync(Guid userId, PlaybackRequest request, CancellationToken ct = default);
}
