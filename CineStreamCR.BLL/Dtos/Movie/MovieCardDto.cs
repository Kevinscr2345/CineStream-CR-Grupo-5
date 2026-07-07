namespace CineStreamCR.BLL.Dtos;

public sealed record MovieCardDto(
    int Id,
    string Title,
    int ReleaseYear,
    int DurationMinutes,
    double Rating,
    string PosterUrl,
    string BackdropUrl,
    bool InAnyWatchList);
