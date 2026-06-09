namespace CineStreamCR.BLL.Dtos;

public sealed record UserDto(Guid Id, string Email, string UserName, string DisplayName);
public sealed record LoginRequest(string Identifier, string Password);
public sealed record GenreDto(int Id, string Name);

public sealed record MovieQuery(
    string? Search,
    int? GenreId,
    int? Year,
    string Sort = "title",
    string Direction = "asc",
    int Page = 1,
    int PageSize = 8);

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

public sealed record MovieCardDto(
    int Id,
    string Title,
    int ReleaseYear,
    int DurationMinutes,
    double Rating,
    string PosterUrl,
    string BackdropUrl,
    bool InAnyWatchList);

public sealed record PersonSummaryDto(int Id, string FullName, string PhotoUrl, string Nationality);
public sealed record CastMemberDto(int Id, string FullName, string PhotoUrl, string? CharacterName);
public sealed record ReviewDto(int Score, string? Comment, DateTime UpdatedAt);

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

public sealed record PersonFilmDto(int MovieId, string Title, string PosterUrl, int ReleaseYear, string Role, string? CharacterName);

public sealed record PersonProfileDto(
    int Id,
    string FullName,
    string Nationality,
    string Biography,
    DateOnly BirthDate,
    string PhotoUrl,
    string InformationSourceUrl,
    string ImageSourceUrl,
    IReadOnlyList<PersonFilmDto> Films);

public sealed record WatchListDto(int Id, string Name, string Description, int MovieCount, DateTime UpdatedAt);
public sealed record WatchListDetailDto(int Id, string Name, string Description, IReadOnlyList<MovieCardDto> Movies, DateTime UpdatedAt);
public sealed record CreateWatchListRequest(string Name, string? Description);
public sealed record UpdateWatchListRequest(string Name, string? Description);
public sealed record ReviewRequest(int Score, string? Comment);
public sealed record PlaybackRequest(int MovieId, double CurrentSecond, double TotalSeconds, bool IsCompleted);
public sealed record PlaybackDto(int MovieId, double CurrentSecond, double TotalSeconds, bool IsCompleted, DateTime LastPlayedAt);

