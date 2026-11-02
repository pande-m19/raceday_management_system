using Microsoft.AspNetCore.Mvc;
using raceday_management_system.MVC.Models;
using raceday_management_system.MVC.Services;
using System.Text.Json;

namespace raceday_management_system.MVC.Controllers
{
    [Route("Organiser")]
    public class organiser_controller : Controller
    {
        private readonly api_service _api_service;

        public organiser_controller(api_service api_service)
        {
            _api_service = api_service;
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser") return RedirectToAction("Login", "Auth");

            var dashboardData = await _api_service.get_async<dashboard_view_model>("dashboard/organiser");
            if (dashboardData == null) dashboardData = new dashboard_view_model();

            return View("~/Views/Organiser/Dashboard.cshtml", dashboardData);
        }

        [HttpGet("CreateEvent")]
        public IActionResult CreateEvent()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser") return RedirectToAction("Login", "Auth");
            return View("~/Views/Organiser/CreateEvent.cshtml");
        }

        [HttpPost("CreateEvent")]
        public async Task<IActionResult> CreateEvent(create_event_view_model model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser") return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid) return View("~/Views/Organiser/CreateEvent.cshtml", model);

            string query = $"events?eventName={Uri.EscapeDataString(model.EventName)}&eventDescription={Uri.EscapeDataString(model.EventDescription)}&eventDate={model.EventDate:yyyy-MM-ddTHH:mm:ss}&location={Uri.EscapeDataString(model.Location)}&distance={model.Distance}&eventType={model.EventType}";

            if (model.MaxParticipants.HasValue) query += $"&maxParticipants={model.MaxParticipants}";
            if (model.EntryFee.HasValue) query += $"&entryFee={model.EntryFee}";

            var result = await _api_service.post_async(query, new { });

            if (result.IsSuccess && result.ResponseContent != null)
            {
                using (JsonDocument doc = JsonDocument.Parse(result.ResponseContent))
                {
                    var newEventId = doc.RootElement.GetProperty("event_id").GetInt32();

                    if (model.BannerFile != null && model.BannerFile.Length > 0)
                    {
                        // CHANGED: Check if the upload succeeds
                        bool uploadSuccess = await _api_service.post_multipart_async($"events/{newEventId}/banner", model.BannerFile);

                        if (!uploadSuccess)
                        {
                            // If the upload fails, show an error but keep the event
                            TempData["ErrorMessage"] = "Event created, but the banner image failed to upload.";
                            return RedirectToAction("Dashboard");
                        }
                    }
                }

                TempData["SuccessMessage"] = "Event created and banner uploaded successfully!";
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError(string.Empty, "Failed to create event. API error.");
            return View("~/Views/Organiser/CreateEvent.cshtml", model);
        }

        [HttpPost("DeleteEvent/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser") return RedirectToAction("Login", "Auth");

            await _api_service.delete_async($"events/{id}");

            TempData["SuccessMessage"] = "Event deleted successfully.";
            return RedirectToAction("Dashboard");
        }

        [HttpGet("EditEvent/{id}")]
        public async Task<IActionResult> EditEvent(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser") return RedirectToAction("Login", "Auth");

            var eventData = await _api_service.get_async<edit_event_view_model>($"events/{id}");

            if (eventData == null) return NotFound();

            return View("~/Views/Organiser/EditEvent.cshtml", eventData);
        }

        [HttpPost("EditEvent/{id}")]
        public async Task<IActionResult> EditEvent(int id, edit_event_view_model model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser") return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid) return View("~/Views/Organiser/EditEvent.cshtml", model);

            string query = $"events/{id}?eventName={Uri.EscapeDataString(model.Event_name)}&eventDescription={Uri.EscapeDataString(model.Event_description)}&eventDate={model.Event_date:yyyy-MM-ddTHH:mm:ss}&location={Uri.EscapeDataString(model.Location)}&distance={model.Distance}&eventType={model.Event_type}&eventStatus={model.Event_status}";

            if (model.Max_participants.HasValue) query += $"&maxParticipants={model.Max_participants}";
            if (model.Entry_fee.HasValue) query += $"&entryFee={model.Entry_fee}";

            await _api_service.put_async(query, new { });

            TempData["SuccessMessage"] = "Event updated successfully!";
            return RedirectToAction("Dashboard");
        }
    }
}