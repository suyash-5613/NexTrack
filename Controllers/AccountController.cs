using Microsoft.AspNetCore.Mvc;
using NexTrack.Models;

namespace NexTrack.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to dashboard
            if (HttpContext.Session.GetString("UserEmail") != null)
            {
                return RedirectToAction("Index", "Items");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Simulate authentication (any valid email + password >= 6 chars)
            HttpContext.Session.SetString("UserEmail", model.Email);
            return RedirectToAction("Index", "Items");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
