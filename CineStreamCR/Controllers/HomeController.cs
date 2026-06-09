using Microsoft.AspNetCore.Mvc;

namespace CineStreamCR.Controllers;

public sealed class HomeController : Controller
{
    [IgnoreAntiforgeryToken]
    public IActionResult Index() => View();

    [IgnoreAntiforgeryToken]
    public IActionResult Error() => View();
}

