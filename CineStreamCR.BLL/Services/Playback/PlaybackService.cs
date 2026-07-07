using AutoMapper;
using CineStreamCR.BLL.Dtos;
using CineStreamCR.DAL.Entidades;
using CineStreamCR.DAL.Repositorios;
using CineStreamCR.DAL.Seguridad;

namespace CineStreamCR.BLL.Services;

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
