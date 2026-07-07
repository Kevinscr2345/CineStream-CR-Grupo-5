namespace CineStreamCR.BLL.Dtos;

public sealed record PersonFilmDto(int MovieId, string Title, string PosterUrl, int ReleaseYear, string Role, string? CharacterName);
