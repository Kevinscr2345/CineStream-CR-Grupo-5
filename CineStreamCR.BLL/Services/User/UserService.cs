using AutoMapper;
using CineStreamCR.BLL.Dtos;
using CineStreamCR.DAL.Entidades;
using CineStreamCR.DAL.Repositorios;
using CineStreamCR.DAL.Seguridad;

namespace CineStreamCR.BLL.Services;

public sealed class UserService(ICineRepositorio repository, IMapper mapper) : IUserService
{
    public async Task<Respuesta<UserDto>> ValidateCredentialsAsync(string identifier, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(password))
            return Respuesta<UserDto>.Error("Debe ingresar usuario/correo y contraseña.");

        var user = await repository.GetUserByIdentifierAsync(identifier, ct);
        if (user is null) return Respuesta<UserDto>.Error("El usuario indicado no existe.", 404);
        if (!PasswordHasher.Verify(password, user.PasswordHash))
            return Respuesta<UserDto>.Error("La contraseña es incorrecta.", 401);

        return Respuesta<UserDto>.Correcta(mapper.Map<UserDto>(user), "Inicio de sesión correcto.");
    }

    public async Task<Respuesta<UserDto>> GetByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await repository.GetUserByIdAsync(userId, ct);
        return user is null
            ? Respuesta<UserDto>.Error("La sesión no corresponde a un usuario válido.", 401)
            : Respuesta<UserDto>.Correcta(mapper.Map<UserDto>(user));
    }
}
