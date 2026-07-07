namespace CineStreamCR.BLL.Dtos;

public sealed record WatchListDetailDto(int Id, string Name, string Description, IReadOnlyList<MovieCardDto> Movies, DateTime UpdatedAt);
