using AutoMapper;
using CineStreamCR.BLL.Dtos;
using CineStreamCR.DAL.Entidades;
using CineStreamCR.DAL.Repositorios;
using CineStreamCR.DAL.Seguridad;

namespace CineStreamCR.BLL.Services;

public sealed class PersonService(ICineRepositorio repository) : IPersonService
{
    public async Task<Respuesta<PersonProfileDto>> GetByIdAsync(int personId, CancellationToken ct = default)
    {
        var person = await repository.GetPersonAsync(personId, ct);
        if (person is null) return Respuesta<PersonProfileDto>.Error("Perfil no encontrado.", 404);
        var films = person.Credits.Where(x => x.Movie.IsActive)
            .OrderByDescending(x => x.Movie.ReleaseYear).ThenBy(x => x.Movie.Title)
            .Select(x => new PersonFilmDto(x.MovieId, x.Movie.Title, x.Movie.PosterUrl, x.Movie.ReleaseYear,
                x.CreditType == CreditType.Director ? "Dirección" : "Actuación", x.CharacterName)).ToList();
        return Respuesta<PersonProfileDto>.Correcta(new PersonProfileDto(
            person.Id, person.FullName, person.Nationality, person.Biography, person.BirthDate,
            person.PhotoUrl, person.InformationSourceUrl, person.ImageSourceUrl, films));
    }
}
