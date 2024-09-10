using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Primex.Models;
using System.Diagnostics;

namespace Primex.Controllers
{
    public class HomeController : Controller
    {
        private readonly PrimexContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, PrimexContext context)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public IActionResult Index()
        {
            ViewBag.Title = "Primex";
            return View();
        }



        public IActionResult Contacts()
        {
            ViewBag.Title = "Контакты";
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Title = "О компании";
            return View();
        }

        public IActionResult Services()
        {
            ViewBag.Title = "Услуги";
            ViewData["IdService"] = new SelectList(_context.Services, "Price", "Service1");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
