using AutoMapper;
using CineStreamCR.BLL.Dtos;
using CineStreamCR.DAL.Entidades;
using CineStreamCR.DAL.Repositorios;
using CineStreamCR.DAL.Seguridad;

namespace CineStreamCR.BLL.Services;

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
