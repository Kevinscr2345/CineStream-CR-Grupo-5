using CineStreamCR.BLL.Dtos;

namespace CineStreamCR.BLL.Services;

public interface IReviewService
{
    Task<Respuesta<ReviewDto>> UpsertAsync(Guid userId, int movieId, ReviewRequest request, CancellationToken ct = default);
}
