using CineStreamCR.DAL.Entidades;
using Microsoft.EntityFrameworkCore;

namespace CineStreamCR.DAL.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<MovieGenre> MovieGenres => Set<MovieGenre>();
    public DbSet<Person> People => Set<Person>();
    public DbSet<MovieCredit> MovieCredits => Set<MovieCredit>();
    public DbSet<WatchList> WatchLists => Set<WatchList>();
    public DbSet<WatchListMovie> WatchListMovies => Set<WatchListMovie>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<PlaybackProgress> PlaybackProgresses => Set<PlaybackProgress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasIndex(x => x.UserName).IsUnique();
            entity.Property(x => x.Email).HasMaxLength(160).IsRequired();
            entity.Property(x => x.UserName).HasMaxLength(80).IsRequired();
            entity.Property(x => x.DisplayName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Title);
            entity.HasIndex(x => x.ReleaseYear);
            entity.Property(x => x.Title).HasMaxLength(180).IsRequired();
            entity.Property(x => x.Synopsis).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.PosterUrl).HasMaxLength(400).IsRequired();
            entity.Property(x => x.BackdropUrl).HasMaxLength(400).IsRequired();
            entity.Property(x => x.VideoUrl).HasMaxLength(400).IsRequired();
            entity.Property(x => x.InformationSourceUrl).HasMaxLength(600).IsRequired();
            entity.Property(x => x.ImageSourceUrl).HasMaxLength(600).IsRequired();
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Movies_ReleaseYear", "[ReleaseYear] BETWEEN 1888 AND 2100");
                t.HasCheckConstraint("CK_Movies_DurationMinutes", "[DurationMinutes] > 0");
            });
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(80).IsRequired();
        });

        modelBuilder.Entity<MovieGenre>(entity =>
        {
            entity.HasKey(x => new { x.MovieId, x.GenreId });
            entity.HasOne(x => x.Movie).WithMany(x => x.MovieGenres).HasForeignKey(x => x.MovieId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Genre).WithMany(x => x.MovieGenres).HasForeignKey(x => x.GenreId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.FullName);
            entity.Property(x => x.FullName).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Nationality).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Biography).HasMaxLength(2500).IsRequired();
            entity.Property(x => x.PhotoUrl).HasMaxLength(600).IsRequired();
            entity.Property(x => x.InformationSourceUrl).HasMaxLength(600).IsRequired();
            entity.Property(x => x.ImageSourceUrl).HasMaxLength(600).IsRequired();
        });

        modelBuilder.Entity<MovieCredit>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.MovieId, x.PersonId, x.CreditType }).IsUnique();
            entity.Property(x => x.CharacterName).HasMaxLength(180);
            entity.HasOne(x => x.Movie).WithMany(x => x.Credits).HasForeignKey(x => x.MovieId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Person).WithMany(x => x.Credits).HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.Restrict);
            entity.ToTable(t => t.HasCheckConstraint("CK_MovieCredits_CreditType", "[CreditType] IN (1, 2)"));
        });

        modelBuilder.Entity<WatchList>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.UserId, x.Name }).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.HasOne(x => x.User).WithMany(x => x.WatchLists).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        });

        modelBuilder.Entity<WatchListMovie>(entity =>
        {
            entity.HasKey(x => new { x.WatchListId, x.MovieId });
            entity.HasOne(x => x.WatchList).WithMany(x => x.Movies).HasForeignKey(x => x.WatchListId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Movie).WithMany(x => x.WatchListMovies).HasForeignKey(x => x.MovieId).OnDelete(DeleteBehavior.Cascade);
            entity.Property(x => x.AddedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.UserId, x.MovieId }).IsUnique();
            entity.Property(x => x.Comment).HasMaxLength(1000);
            entity.HasOne(x => x.User).WithMany(x => x.Reviews).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Movie).WithMany(x => x.Reviews).HasForeignKey(x => x.MovieId).OnDelete(DeleteBehavior.Cascade);
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.ToTable(t => t.HasCheckConstraint("CK_Reviews_Score", "[Score] BETWEEN 1 AND 10"));
        });

        modelBuilder.Entity<PlaybackProgress>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.UserId, x.MovieId }).IsUnique();
            entity.HasOne(x => x.User).WithMany(x => x.PlaybackProgresses).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Movie).WithMany(x => x.PlaybackProgresses).HasForeignKey(x => x.MovieId).OnDelete(DeleteBehavior.Cascade);
            entity.Property(x => x.LastPlayedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_PlaybackProgress_CurrentSecond", "[CurrentSecond] >= 0");
                t.HasCheckConstraint("CK_PlaybackProgress_TotalSeconds", "[TotalSeconds] >= 0");
            });
        });
    }
}

