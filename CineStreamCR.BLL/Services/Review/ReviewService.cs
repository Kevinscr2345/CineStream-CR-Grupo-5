using AutoMapper;
using CineStreamCR.BLL.Dtos;
using CineStreamCR.DAL.Entidades;
using CineStreamCR.DAL.Repositorios;
using CineStreamCR.DAL.Seguridad;

namespace CineStreamCR.BLL.Services;

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
