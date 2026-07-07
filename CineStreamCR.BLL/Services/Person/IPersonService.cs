using CineStreamCR.BLL.Dtos;

namespace CineStreamCR.BLL.Services;

public interface IPersonService
{
    Task<Respuesta<PersonProfileDto>> GetByIdAsync(int personId, CancellationToken ct = default);
}
