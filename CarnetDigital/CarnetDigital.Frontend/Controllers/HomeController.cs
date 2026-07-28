using CarnetDigital.Frontend.Filters;
using CarnetDigital.Frontend.Models;
using CarnetDigital.Frontend.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CarnetDigital.Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITokenProvider _tokenProvider;

        public HomeController(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        [RequiereSesion]
        public IActionResult Index()
        {
            ViewBag.Username = _tokenProvider.GetUsername();
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
