namespace CineStreamCR.BLL.Dtos;

public sealed record PlaybackDto(int MovieId, double CurrentSecond, double TotalSeconds, bool IsCompleted, DateTime LastPlayedAt);
