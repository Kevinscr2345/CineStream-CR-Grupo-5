using CineStreamCR.BLL.Dtos;

namespace CineStreamCR.BLL.Services;

public interface IUserService
{
    Task<Respuesta<UserDto>> ValidateCredentialsAsync(string identifier, string password, CancellationToken ct = default);
    Task<Respuesta<UserDto>> GetByIdAsync(Guid userId, CancellationToken ct = default);
}
