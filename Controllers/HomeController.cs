using Microsoft.AspNetCore.Mvc;

namespace MeerkatMvc.Controllers;

public class HomeController : Controller
{
    public IActionResult Index(string message = "Hello, world!")
    {
        ViewBag.Message = message;
        return View();
    }
}
