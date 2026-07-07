namespace CineStreamCR.BLL.Dtos;

public sealed record PlaybackRequest(int MovieId, double CurrentSecond, double TotalSeconds, bool IsCompleted);
