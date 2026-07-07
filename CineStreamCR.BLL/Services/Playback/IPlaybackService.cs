using CineStreamCR.BLL.Dtos;

namespace CineStreamCR.BLL.Services;

public interface IPlaybackService
{
    Task<Respuesta<PlaybackDto?>> GetAsync(Guid userId, int movieId, CancellationToken ct = default);
    Task<Respuesta<PlaybackDto>> SaveAsync(Guid userId, PlaybackRequest request, CancellationToken ct = default);
}
