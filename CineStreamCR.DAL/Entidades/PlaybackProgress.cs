namespace CineStreamCR.DAL.Entidades;

public sealed class PlaybackProgress
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public double CurrentSecond { get; set; }
    public double TotalSeconds { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime LastPlayedAt { get; set; } = DateTime.UtcNow;
}
