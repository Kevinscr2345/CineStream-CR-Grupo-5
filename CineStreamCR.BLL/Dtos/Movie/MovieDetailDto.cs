namespace CineStreamCR.BLL.Dtos;

public sealed record MovieDetailDto(
    int Id,
    string Title,
    string Synopsis,
    int ReleaseYear,
    int DurationMinutes,
    double Rating,
    string PosterUrl,
    string BackdropUrl,
    string VideoUrl,
    string InformationSourceUrl,
    string ImageSourceUrl,
    IReadOnlyList<string> Genres,
    IReadOnlyList<PersonSummaryDto> Directors,
    IReadOnlyList<CastMemberDto> Cast,
    IReadOnlyList<int> InWatchListIds,
    ReviewDto? UserReview);
