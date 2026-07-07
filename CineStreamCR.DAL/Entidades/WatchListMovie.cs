namespace CineStreamCR.DAL.Entidades;

public sealed class WatchListMovie
{
    public int WatchListId { get; set; }
    public WatchList WatchList { get; set; } = null!;
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
