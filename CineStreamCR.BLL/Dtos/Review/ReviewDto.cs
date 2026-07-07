namespace CineStreamCR.BLL.Dtos;

public sealed record ReviewDto(int Score, string? Comment, DateTime UpdatedAt);
