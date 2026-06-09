using CineStreamCR.DAL.Data;
using CineStreamCR.DAL.Entidades;
using Microsoft.EntityFrameworkCore;

namespace CineStreamCR.DAL.Repositorios;

public sealed class CineRepositorio(ApplicationDbContext context) : ICineRepositorio
{
    public async Task<User?> GetUserByIdentifierAsync(string identifier, CancellationToken ct = default)
    {
        identifier = (identifier ?? string.Empty).Trim().ToLowerInvariant();
        return await context.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email.ToLower() == identifier || x.UserName.ToLower() == identifier, ct);
    }

    public Task<User?> GetUserByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Genre>> GetGenresAsync(CancellationToken ct = default) =>
        await context.Genres.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);

    public async Task<IReadOnlyList<int>> GetYearsAsync(CancellationToken ct = default) =>
        await context.Movies.AsNoTracking().Where(x => x.IsActive).Select(x => x.ReleaseYear)
            .Distinct().OrderByDescending(x => x).ToListAsync(ct);

    public async Task<(IReadOnlyList<Movie> Items, int TotalCount, int Page, int PageSize, int TotalPages)> GetCatalogAsync(
        Guid userId, string? search, int? genreId, int? year, string sort, string direction, int page, int pageSize, CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 4, 24);
        var query = context.Movies.AsNoTracking()
            .Include(x => x.Reviews)
            .Include(x => x.WatchListMovies).ThenInclude(x => x.WatchList)
            .Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var clean = search.Trim();
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{clean}%"));
        }
        if (genreId is > 0)
            query = query.Where(x => x.MovieGenres.Any(g => g.GenreId == genreId));
        if (year is > 0)
            query = query.Where(x => x.ReleaseYear == year);

        var descending = string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase);
        query = (sort ?? "title").ToLowerInvariant() switch
        {
            "year" => descending ? query.OrderByDescending(x => x.ReleaseYear).ThenBy(x => x.Title) : query.OrderBy(x => x.ReleaseYear).ThenBy(x => x.Title),
            "rating" => descending
                ? query.OrderByDescending(x => x.Reviews.Any() ? x.Reviews.Average(r => r.Score) : 0).ThenBy(x => x.Title)
                : query.OrderBy(x => x.Reviews.Any() ? x.Reviews.Average(r => r.Score) : 0).ThenBy(x => x.Title),
            _ => descending ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title)
        };

        var totalCount = await query.CountAsync(ct);
        var totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Min(page, totalPages);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).AsSplitQuery().ToListAsync(ct);
        return (items, totalCount, page, pageSize, totalPages);
    }

    public Task<Movie?> GetMovieDetailAsync(int id, CancellationToken ct = default) =>
        context.Movies.AsNoTracking().AsSplitQuery()
            .Include(x => x.MovieGenres).ThenInclude(x => x.Genre)
            .Include(x => x.Credits).ThenInclude(x => x.Person)
            .Include(x => x.Reviews)
            .Include(x => x.WatchListMovies).ThenInclude(x => x.WatchList)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);

    public Task<Movie?> GetFeaturedMovieAsync(CancellationToken ct = default) =>
        context.Movies.AsNoTracking().AsSplitQuery()
            .Include(x => x.Reviews)
            .Include(x => x.WatchListMovies).ThenInclude(x => x.WatchList)
            .Where(x => x.IsActive && x.IsFeatured)
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(ct);

    public Task<bool> MovieExistsAsync(int id, CancellationToken ct = default) =>
        context.Movies.AnyAsync(x => x.Id == id && x.IsActive, ct);

    public Task<Person?> GetPersonAsync(int id, CancellationToken ct = default) =>
        context.People.AsNoTracking().AsSplitQuery()
            .Include(x => x.Credits).ThenInclude(x => x.Movie)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<WatchList>> GetWatchListsAsync(Guid userId, CancellationToken ct = default) =>
        await context.WatchLists.AsNoTracking().Include(x => x.Movies)
            .Where(x => x.UserId == userId).OrderByDescending(x => x.UpdatedAt).ToListAsync(ct);

    public Task<WatchList?> GetWatchListAsync(Guid userId, int id, CancellationToken ct = default) =>
        context.WatchLists.AsNoTracking().AsSplitQuery()
            .Include(x => x.Movies).ThenInclude(x => x.Movie).ThenInclude(x => x.Reviews)
            .Include(x => x.Movies).ThenInclude(x => x.Movie).ThenInclude(x => x.WatchListMovies).ThenInclude(x => x.WatchList)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);

    public Task<bool> WatchListNameExistsAsync(Guid userId, string name, int? excludeId = null, CancellationToken ct = default) =>
        context.WatchLists.AnyAsync(x => x.UserId == userId && x.Name.ToLower() == name.ToLower() && (!excludeId.HasValue || x.Id != excludeId.Value), ct);

    public async Task<WatchList> CreateWatchListAsync(WatchList list, CancellationToken ct = default)
    {
        context.WatchLists.Add(list);
        await context.SaveChangesAsync(ct);
        return list;
    }

    public async Task<bool> UpdateWatchListAsync(WatchList list, CancellationToken ct = default)
    {
        var entity = await context.WatchLists.FirstOrDefaultAsync(x => x.Id == list.Id && x.UserId == list.UserId, ct);
        if (entity is null) return false;
        entity.Name = list.Name;
        entity.Description = list.Description;
        entity.UpdatedAt = DateTime.UtcNow;
        return await context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteWatchListAsync(Guid userId, int id, CancellationToken ct = default)
    {
        var entity = await context.WatchLists.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);
        if (entity is null) return false;
        context.WatchLists.Remove(entity);
        return await context.SaveChangesAsync(ct) > 0;
    }

    public Task<bool> WatchListMovieExistsAsync(Guid userId, int watchListId, int movieId, CancellationToken ct = default) =>
        context.WatchListMovies.AnyAsync(x => x.WatchListId == watchListId && x.MovieId == movieId && x.WatchList.UserId == userId, ct);

    public async Task<bool> AddMovieToWatchListAsync(Guid userId, int watchListId, int movieId, CancellationToken ct = default)
    {
        var list = await context.WatchLists.FirstOrDefaultAsync(x => x.Id == watchListId && x.UserId == userId, ct);
        if (list is null || !await MovieExistsAsync(movieId, ct)) return false;
        if (await WatchListMovieExistsAsync(userId, watchListId, movieId, ct)) return false;
        context.WatchListMovies.Add(new WatchListMovie { WatchListId = watchListId, MovieId = movieId });
        list.UpdatedAt = DateTime.UtcNow;
        return await context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> RemoveMovieFromWatchListAsync(Guid userId, int watchListId, int movieId, CancellationToken ct = default)
    {
        var item = await context.WatchListMovies.Include(x => x.WatchList)
            .FirstOrDefaultAsync(x => x.WatchListId == watchListId && x.MovieId == movieId && x.WatchList.UserId == userId, ct);
        if (item is null) return false;
        item.WatchList.UpdatedAt = DateTime.UtcNow;
        context.WatchListMovies.Remove(item);
        return await context.SaveChangesAsync(ct) > 0;
    }

    public Task<Review?> GetReviewAsync(Guid userId, int movieId, CancellationToken ct = default) =>
        context.Reviews.FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId, ct);

    public async Task<Review> SaveReviewAsync(Review review, CancellationToken ct = default)
    {
        if (review.Id == 0) context.Reviews.Add(review);
        await context.SaveChangesAsync(ct);
        return review;
    }

    public Task<PlaybackProgress?> GetPlaybackAsync(Guid userId, int movieId, CancellationToken ct = default) =>
        context.PlaybackProgresses.FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId, ct);

    public async Task<PlaybackProgress> SavePlaybackAsync(PlaybackProgress progress, CancellationToken ct = default)
    {
        if (progress.Id == 0) context.PlaybackProgresses.Add(progress);
        await context.SaveChangesAsync(ct);
        return progress;
    }
}
