using AutoMapper;
using CineStreamCR.BLL.Dtos;
using CineStreamCR.DAL.Entidades;
using CineStreamCR.DAL.Repositorios;
using CineStreamCR.DAL.Seguridad;

namespace CineStreamCR.BLL.Services;

public sealed class MediaService(ICineRepositorio repository, IMapper mapper) : IMovieService
{
    public async Task<Respuesta<IReadOnlyList<GenreDto>>> GetGenresAsync(CancellationToken ct = default)
    {
        var genres = await repository.GetGenresAsync(ct);
        var data = genres.Select(x => mapper.Map<GenreDto>(x)).ToList();
        return Respuesta<IReadOnlyList<GenreDto>>.Correcta(data);
    }

    public async Task<Respuesta<IReadOnlyList<int>>> GetYearsAsync(CancellationToken ct = default) =>
        Respuesta<IReadOnlyList<int>>.Correcta(await repository.GetYearsAsync(ct));

    public async Task<Respuesta<PagedResult<MovieCardDto>>> GetCatalogAsync(Guid userId, MovieQuery query, CancellationToken ct = default)
    {
        var result = await repository.GetCatalogAsync(userId, query.Search, query.GenreId, query.Year,
            query.Sort, query.Direction, query.Page, query.PageSize, ct);
        var items = result.Items.Select(x => ToCard(x, userId)).ToList();
        return Respuesta<PagedResult<MovieCardDto>>.Correcta(
            new PagedResult<MovieCardDto>(items, result.Page, result.PageSize, result.TotalCount, result.TotalPages));
    }

    public async Task<Respuesta<MovieDetailDto>> GetByIdAsync(Guid userId, int movieId, CancellationToken ct = default)
    {
        var movie = await repository.GetMovieDetailAsync(movieId, ct);
        if (movie is null) return Respuesta<MovieDetailDto>.Error("Película no encontrada.", 404);

        var directors = movie.Credits.Where(x => x.CreditType == CreditType.Director)
            .OrderBy(x => x.BillingOrder)
            .Select(x => new PersonSummaryDto(x.Person.Id, x.Person.FullName, x.Person.PhotoUrl, x.Person.Nationality))
            .ToList();
        var cast = movie.Credits.Where(x => x.CreditType == CreditType.Actor).OrderBy(x => x.BillingOrder)
            .Select(x => new CastMemberDto(x.Person.Id, x.Person.FullName, x.Person.PhotoUrl, x.CharacterName)).ToList();
        var review = movie.Reviews.FirstOrDefault(x => x.UserId == userId);
        var average = movie.Reviews.Count == 0 ? 0 : Math.Round(movie.Reviews.Average(x => x.Score), 1);

        var dto = new MovieDetailDto(
            movie.Id, movie.Title, movie.Synopsis, movie.ReleaseYear, movie.DurationMinutes, average,
            movie.PosterUrl, movie.BackdropUrl, movie.VideoUrl, movie.InformationSourceUrl, movie.ImageSourceUrl,
            movie.MovieGenres.Select(x => x.Genre.Name).OrderBy(x => x).ToList(), directors, cast,
            movie.WatchListMovies.Where(x => x.WatchList.UserId == userId).Select(x => x.WatchListId).ToList(),
            review is null ? null : new ReviewDto(review.Score, review.Comment, review.UpdatedAt));
        return Respuesta<MovieDetailDto>.Correcta(dto);
    }

    public async Task<Respuesta<MovieCardDto>> GetFeaturedAsync(Guid userId, CancellationToken ct = default)
    {
        var movie = await repository.GetFeaturedMovieAsync(ct);
        return movie is null
            ? Respuesta<MovieCardDto>.Error("No existe una película destacada.", 404)
            : Respuesta<MovieCardDto>.Correcta(ToCard(movie, userId));
    }

    private static MovieCardDto ToCard(Movie movie, Guid userId) => new(
        movie.Id, movie.Title, movie.ReleaseYear, movie.DurationMinutes,
        movie.Reviews.Count == 0 ? 0 : Math.Round(movie.Reviews.Average(x => x.Score), 1),
        movie.PosterUrl, movie.BackdropUrl,
        movie.WatchListMovies.Any(x => x.WatchList.UserId == userId));
}
