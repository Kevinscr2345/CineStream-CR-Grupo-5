namespace CineStreamCR.BLL.Dtos;

public sealed record WatchListDto(int Id, string Name, string Description, int MovieCount, DateTime UpdatedAt);
