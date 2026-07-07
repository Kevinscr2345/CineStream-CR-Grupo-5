namespace CineStreamCR.BLL.Dtos;

public sealed record MovieQuery(
    string? Search,
    int? GenreId,
    int? Year,
    string Sort = "title",
    string Direction = "asc",
    int Page = 1,
    int PageSize = 8);
