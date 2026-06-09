namespace CineStreamCR.DAL.Entidades;

public enum CreditType
{
    Director = 1,
    Actor = 2
}

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

public sealed class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
}

public sealed class MovieGenre
{
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public int GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
}

public sealed class Person
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string InformationSourceUrl { get; set; } = string.Empty;
    public string ImageSourceUrl { get; set; } = string.Empty;
    public ICollection<MovieCredit> Credits { get; set; } = new List<MovieCredit>();
}

public sealed class MovieCredit
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;
    public CreditType CreditType { get; set; }
    public string? CharacterName { get; set; }
    public int BillingOrder { get; set; }
}

public sealed class WatchList
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<WatchListMovie> Movies { get; set; } = new List<WatchListMovie>();
}

public sealed class WatchListMovie
{
    public int WatchListId { get; set; }
    public WatchList WatchList { get; set; } = null!;
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}

public sealed class Review
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public int Score { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

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
