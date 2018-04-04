using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GraphSample.Models;
using Microsoft.Extensions.Caching.Memory;

namespace GraphSample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var v = ServiceHelper.Caching.Get("signedIn");
            ViewBag.SignIn = (bool?) v ?? false;
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
