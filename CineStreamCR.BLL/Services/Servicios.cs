using AutoMapper;
using CineStreamCR.BLL.Dtos;
using CineStreamCR.DAL.Entidades;
using CineStreamCR.DAL.Repositorios;
using CineStreamCR.DAL.Seguridad;

namespace CineStreamCR.BLL.Services;

public sealed class UserService(ICineRepositorio repository, IMapper mapper) : IUserService
{
    public async Task<Respuesta<UserDto>> ValidateCredentialsAsync(string identifier, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(password))
            return Respuesta<UserDto>.Error("Debe ingresar usuario/correo y contraseña.");

        var user = await repository.GetUserByIdentifierAsync(identifier, ct);
        if (user is null) return Respuesta<UserDto>.Error("El usuario indicado no existe.", 404);
        if (!PasswordHasher.Verify(password, user.PasswordHash))
            return Respuesta<UserDto>.Error("La contraseña es incorrecta.", 401);

        return Respuesta<UserDto>.Correcta(mapper.Map<UserDto>(user), "Inicio de sesión correcto.");
    }

    public async Task<Respuesta<UserDto>> GetByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await repository.GetUserByIdAsync(userId, ct);
        return user is null
            ? Respuesta<UserDto>.Error("La sesión no corresponde a un usuario válido.", 401)
            : Respuesta<UserDto>.Correcta(mapper.Map<UserDto>(user));
    }
}

public sealed class MovieService(ICineRepositorio repository, IMapper mapper) : IMovieService
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

public sealed class PersonService(ICineRepositorio repository) : IPersonService
{
    public async Task<Respuesta<PersonProfileDto>> GetByIdAsync(int personId, CancellationToken ct = default)
    {
        var person = await repository.GetPersonAsync(personId, ct);
        if (person is null) return Respuesta<PersonProfileDto>.Error("Perfil no encontrado.", 404);
        var films = person.Credits.Where(x => x.Movie.IsActive)
            .OrderByDescending(x => x.Movie.ReleaseYear).ThenBy(x => x.Movie.Title)
            .Select(x => new PersonFilmDto(x.MovieId, x.Movie.Title, x.Movie.PosterUrl, x.Movie.ReleaseYear,
                x.CreditType == CreditType.Director ? "Dirección" : "Actuación", x.CharacterName)).ToList();
        return Respuesta<PersonProfileDto>.Correcta(new PersonProfileDto(
            person.Id, person.FullName, person.Nationality, person.Biography, person.BirthDate,
            person.PhotoUrl, person.InformationSourceUrl, person.ImageSourceUrl, films));
    }
}

public sealed class WatchListService(ICineRepositorio repository) : IWatchListService
{
    public async Task<Respuesta<IReadOnlyList<WatchListDto>>> GetAllAsync(Guid userId, CancellationToken ct = default)
    {
        var lists = await repository.GetWatchListsAsync(userId, ct);
        var dto = lists.Select(x => new WatchListDto(x.Id, x.Name, x.Description, x.Movies.Count, x.UpdatedAt)).ToList();
        return Respuesta<IReadOnlyList<WatchListDto>>.Correcta(dto);
    }

    public async Task<Respuesta<WatchListDetailDto>> GetByIdAsync(Guid userId, int watchListId, CancellationToken ct = default)
    {
        var list = await repository.GetWatchListAsync(userId, watchListId, ct);
        if (list is null) return Respuesta<WatchListDetailDto>.Error("Lista no encontrada.", 404);
        var movies = list.Movies.OrderByDescending(x => x.AddedAt).Select(x => new MovieCardDto(
            x.Movie.Id, x.Movie.Title, x.Movie.ReleaseYear, x.Movie.DurationMinutes,
            x.Movie.Reviews.Count == 0 ? 0 : Math.Round(x.Movie.Reviews.Average(r => r.Score), 1),
            x.Movie.PosterUrl, x.Movie.BackdropUrl,
            x.Movie.WatchListMovies.Any(w => w.WatchList.UserId == userId))).ToList();
        return Respuesta<WatchListDetailDto>.Correcta(new WatchListDetailDto(list.Id, list.Name, list.Description, movies, list.UpdatedAt));
    }

    public async Task<Respuesta<WatchListDto>> CreateAsync(Guid userId, CreateWatchListRequest request, CancellationToken ct = default)
    {
        var name = (request.Name ?? string.Empty).Trim();
        var description = (request.Description ?? string.Empty).Trim();
        if (name.Length is < 2 or > 100) return Respuesta<WatchListDto>.Error("El nombre debe tener entre 2 y 100 caracteres.");
        if (description.Length > 500) return Respuesta<WatchListDto>.Error("La descripción no puede superar 500 caracteres.");
        if (await repository.WatchListNameExistsAsync(userId, name, null, ct)) return Respuesta<WatchListDto>.Error("Ya existe una lista con ese nombre.");
        var list = await repository.CreateWatchListAsync(new WatchList { UserId = userId, Name = name, Description = description }, ct);
        return Respuesta<WatchListDto>.Correcta(new WatchListDto(list.Id, list.Name, list.Description, 0, list.UpdatedAt), "Lista creada correctamente.", 201);
    }

    public async Task<Respuesta<object>> UpdateAsync(Guid userId, int watchListId, UpdateWatchListRequest request, CancellationToken ct = default)
    {
        var name = (request.Name ?? string.Empty).Trim();
        var description = (request.Description ?? string.Empty).Trim();
        if (name.Length is < 2 or > 100) return Respuesta<object>.Error("El nombre debe tener entre 2 y 100 caracteres.");
        if (description.Length > 500) return Respuesta<object>.Error("La descripción no puede superar 500 caracteres.");
        if (await repository.WatchListNameExistsAsync(userId, name, watchListId, ct)) return Respuesta<object>.Error("Ya existe otra lista con ese nombre.");
        var ok = await repository.UpdateWatchListAsync(new WatchList { Id = watchListId, UserId = userId, Name = name, Description = description }, ct);
        return ok ? Respuesta<object>.Correcta(null, "Lista actualizada correctamente.") : Respuesta<object>.Error("La lista no existe.", 404);
    }

    public async Task<Respuesta<object>> DeleteAsync(Guid userId, int watchListId, CancellationToken ct = default) =>
        await repository.DeleteWatchListAsync(userId, watchListId, ct)
            ? Respuesta<object>.Correcta(null, "Lista eliminada correctamente.")
            : Respuesta<object>.Error("La lista no existe.", 404);

    public async Task<Respuesta<object>> AddMovieAsync(Guid userId, int watchListId, int movieId, CancellationToken ct = default)
    {
        if (!await repository.MovieExistsAsync(movieId, ct)) return Respuesta<object>.Error("La película no existe.", 404);
        if (await repository.WatchListMovieExistsAsync(userId, watchListId, movieId, ct)) return Respuesta<object>.Error("La película ya está en esta lista.");
        return await repository.AddMovieToWatchListAsync(userId, watchListId, movieId, ct)
            ? Respuesta<object>.Correcta(null, "Película agregada a la lista.")
            : Respuesta<object>.Error("No fue posible agregar la película. Verifique la lista.", 404);
    }

    public async Task<Respuesta<object>> RemoveMovieAsync(Guid userId, int watchListId, int movieId, CancellationToken ct = default) =>
        await repository.RemoveMovieFromWatchListAsync(userId, watchListId, movieId, ct)
            ? Respuesta<object>.Correcta(null, "Película eliminada de la lista.")
            : Respuesta<object>.Error("La película no estaba en la lista.", 404);
}

public sealed class ReviewService(ICineRepositorio repository) : IReviewService
{
    public async Task<Respuesta<ReviewDto>> UpsertAsync(Guid userId, int movieId, ReviewRequest request, CancellationToken ct = default)
    {
        if (request.Score is < 1 or > 10) return Respuesta<ReviewDto>.Error("La calificación debe estar entre 1 y 10.");
        if ((request.Comment?.Length ?? 0) > 1000) return Respuesta<ReviewDto>.Error("La reseña no puede superar 1000 caracteres.");
        if (!await repository.MovieExistsAsync(movieId, ct)) return Respuesta<ReviewDto>.Error("La película no existe.", 404);

        var review = await repository.GetReviewAsync(userId, movieId, ct);
        if (review is null)
            review = new Review { UserId = userId, MovieId = movieId };
        review.Score = request.Score;
        review.Comment = request.Comment?.Trim();
        review.UpdatedAt = DateTime.UtcNow;
        await repository.SaveReviewAsync(review, ct);
        return Respuesta<ReviewDto>.Correcta(new ReviewDto(review.Score, review.Comment, review.UpdatedAt), "Calificación guardada correctamente.");
    }
}

public sealed class PlaybackService(ICineRepositorio repository) : IPlaybackService
{
    public async Task<Respuesta<PlaybackDto?>> GetAsync(Guid userId, int movieId, CancellationToken ct = default)
    {
        var item = await repository.GetPlaybackAsync(userId, movieId, ct);
        var dto = item is null ? null : new PlaybackDto(item.MovieId, item.CurrentSecond, item.TotalSeconds, item.IsCompleted, item.LastPlayedAt);
        return Respuesta<PlaybackDto?>.Correcta(dto);
    }

    public async Task<Respuesta<PlaybackDto>> SaveAsync(Guid userId, PlaybackRequest request, CancellationToken ct = default)
    {
        if (!await repository.MovieExistsAsync(request.MovieId, ct)) return Respuesta<PlaybackDto>.Error("La película no existe.", 404);
        var progress = await repository.GetPlaybackAsync(userId, request.MovieId, ct)
            ?? new PlaybackProgress { UserId = userId, MovieId = request.MovieId };
        progress.CurrentSecond = Math.Max(0, request.CurrentSecond);
        progress.TotalSeconds = Math.Max(0, request.TotalSeconds);
        progress.IsCompleted = request.IsCompleted;
        progress.LastPlayedAt = DateTime.UtcNow;
        await repository.SavePlaybackAsync(progress, ct);
        return Respuesta<PlaybackDto>.Correcta(new PlaybackDto(progress.MovieId, progress.CurrentSecond, progress.TotalSeconds, progress.IsCompleted, progress.LastPlayedAt));
    }
}
