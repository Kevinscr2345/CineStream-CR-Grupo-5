using AutoMapper;
using CineStreamCR.BLL.Dtos;
using CineStreamCR.DAL.Entidades;

namespace CineStreamCR.BLL;

public sealed class MapeoClases : Profile
{
    public MapeoClases()
    {
        CreateMap<User, UserDto>();
        CreateMap<Genre, GenreDto>();
    }
}
