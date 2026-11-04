using Microsoft.AspNetCore.Mvc;
using raceday_management_system.MVC.Models;
using raceday_management_system.MVC.Services;

namespace raceday_management_system.MVC.Controllers
{//start of namespace

    [Route("events")]
    public class events_controller : Controller
    {//start of class

        private readonly api_service _api_service;

        public events_controller(api_service api_service)
        {//start of constructor
            _api_service = api_service;
        }//end of constructor

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {//start of Details()

            // 1. Fetch the event details
            var eventData = await _api_service.get_async<event_details_view_model>($"events/{id}");
            if (eventData == null) return NotFound();

            // 2. Fetch the categories for this event
            var categories = await _api_service.get_async<List<category_view_model>>($"events/{id}/categories");
            eventData.Categories = categories ?? new List<category_view_model>();

            // 3. Check if the logged-in user is already enrolled
            if (HttpContext.Session.GetString("Role") == "Participant")
            {
                // Use strongly typed model to prevent the dynamic JSON exception
                var enrolmentStatus = await _api_service.get_async<enrolment_status_view_model>($"enrolments/me/{id}");

                if (enrolmentStatus != null && enrolmentStatus.IsEnrolled)
                {
                    eventData.IsEnrolled = true;
                }
            }

            return View("~/Views/Events/Details.cshtml", eventData);

        }//end of Details()

        [HttpPost("Enrol/{id}")]
        public async Task<IActionResult> Enrol(int id, int categoryId)
        {//start of Enrol()

            if (HttpContext.Session.GetString("Role") != "Participant")
            {
                TempData["ErrorMessage"] = "Only participants can enrol in events.";
                return RedirectToAction("Details", new { id = id });
            }

            // Call the API to enrol
            string endpoint = $"enrolments/{id}?categoryId={categoryId}";
            var result = await _api_service.post_async(endpoint, new { });

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Successfully enrolled!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to enrol. You might already be enrolled.";
            }

            return RedirectToAction("Details", new { id = id });

        }//end of Enrol()

    }//end of class

}//end of namespace