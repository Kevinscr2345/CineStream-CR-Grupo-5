using CineStreamCR.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineStreamCR.Controllers;

[Authorize]
[Route("api/people")]
public sealed class PersonController(IPersonService personService) : ApiControllerBase
{
    [HttpGet("{id:int}")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Get(int id, CancellationToken ct) => FromResponse(await personService.GetByIdAsync(id, ct));
}

