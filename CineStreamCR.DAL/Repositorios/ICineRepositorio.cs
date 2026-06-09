using CineStreamCR.DAL.Entidades;

namespace CineStreamCR.DAL.Repositorios;

public interface ICineRepositorio
{
    Task<User?> GetUserByIdentifierAsync(string identifier, CancellationToken ct = default);
    Task<User?> GetUserByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<Genre>> GetGenresAsync(CancellationToken ct = default);
    Task<IReadOnlyList<int>> GetYearsAsync(CancellationToken ct = default);
    Task<(IReadOnlyList<Movie> Items, int TotalCount, int Page, int PageSize, int TotalPages)> GetCatalogAsync(
        Guid userId, string? search, int? genreId, int? year, string sort, string direction, int page, int pageSize, CancellationToken ct = default);
    Task<Movie?> GetMovieDetailAsync(int id, CancellationToken ct = default);
    Task<Movie?> GetFeaturedMovieAsync(CancellationToken ct = default);
    Task<bool> MovieExistsAsync(int id, CancellationToken ct = default);

    Task<Person?> GetPersonAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<WatchList>> GetWatchListsAsync(Guid userId, CancellationToken ct = default);
    Task<WatchList?> GetWatchListAsync(Guid userId, int id, CancellationToken ct = default);
    Task<bool> WatchListNameExistsAsync(Guid userId, string name, int? excludeId = null, CancellationToken ct = default);
    Task<WatchList> CreateWatchListAsync(WatchList list, CancellationToken ct = default);
    Task<bool> UpdateWatchListAsync(WatchList list, CancellationToken ct = default);
    Task<bool> DeleteWatchListAsync(Guid userId, int id, CancellationToken ct = default);
    Task<bool> WatchListMovieExistsAsync(Guid userId, int watchListId, int movieId, CancellationToken ct = default);
    Task<bool> AddMovieToWatchListAsync(Guid userId, int watchListId, int movieId, CancellationToken ct = default);
    Task<bool> RemoveMovieFromWatchListAsync(Guid userId, int watchListId, int movieId, CancellationToken ct = default);

    Task<Review?> GetReviewAsync(Guid userId, int movieId, CancellationToken ct = default);
    Task<Review> SaveReviewAsync(Review review, CancellationToken ct = default);

    Task<PlaybackProgress?> GetPlaybackAsync(Guid userId, int movieId, CancellationToken ct = default);
    Task<PlaybackProgress> SavePlaybackAsync(PlaybackProgress progress, CancellationToken ct = default);
}

