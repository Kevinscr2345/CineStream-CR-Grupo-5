namespace CineStreamCR.DAL.Entidades;

public sealed class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Synopsis { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public int DurationMinutes { get; set; }
    public string PosterUrl { get; set; } = string.Empty;
    public string BackdropUrl { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public string InformationSourceUrl { get; set; } = string.Empty;
    public string ImageSourceUrl { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    public ICollection<MovieCredit> Credits { get; set; } = new List<MovieCredit>();
    public ICollection<WatchListMovie> WatchListMovies { get; set; } = new List<WatchListMovie>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<PlaybackProgress> PlaybackProgresses { get; set; } = new List<PlaybackProgress>();
}
