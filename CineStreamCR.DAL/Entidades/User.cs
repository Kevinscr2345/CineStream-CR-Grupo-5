namespace CineStreamCR.DAL.Entidades;

public sealed class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<WatchList> WatchLists { get; set; } = new List<WatchList>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<PlaybackProgress> PlaybackProgresses { get; set; } = new List<PlaybackProgress>();
}
