namespace CineStreamCR.BLL.Dtos;

public sealed record PersonProfileDto(
    int Id,
    string FullName,
    string Nationality,
    string Biography,
    DateOnly BirthDate,
    string PhotoUrl,
    string InformationSourceUrl,
    string ImageSourceUrl,
    IReadOnlyList<PersonFilmDto> Films);
