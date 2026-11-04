using Microsoft.AspNetCore.Mvc;
using raceday_management_system.MVC.Models;
using raceday_management_system.MVC.Services;

namespace raceday_management_system.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly api_service _api_service;

        public HomeController(api_service api_service)
        {
            _api_service = api_service;
        }

        public async Task<IActionResult> Index()
        {
            // If the user is not logged in, show the homepage but without events (or redirect to login)
            if (HttpContext.Session.GetString("User_id") == null)
            {
                return View(new List<event_card_view_model>());
            }

            // Fetch upcoming events from the API
            var events = await _api_service.get_async<List<event_card_view_model>>("events/upcoming");

            return View(events ?? new List<event_card_view_model>());
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}