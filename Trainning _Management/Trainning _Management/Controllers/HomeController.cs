using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Trainning__Management.Models;

namespace Trainning__Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
     
        }
        public IActionResult I_Index()
        {
            return View();
        }

        public IActionResult TIndex()
        {
            return View();
        }
        public IActionResult AIndex()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
