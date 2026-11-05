using Microsoft.AspNetCore.Mvc;
using raceday_management_system.MVC.Models;
using raceday_management_system.MVC.Services;
using System.Text.Json;

namespace raceday_management_system.MVC.Controllers
{//start of namespace

    [Route("Organiser")]
    public class organiser_controller : Controller
    {//start of class

        private readonly api_service _api_service;

        public organiser_controller(api_service api_service)
        {//start of constructor
            _api_service = api_service;
        }//end of constructor

        //dashboard
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {//start of Dashboard()

            if (HttpContext.Session.GetString("Role") != "Organiser") return RedirectToAction("Login", "Auth");

            var dashboardData = await _api_service.get_async<dashboard_view_model>("dashboard/organiser");
            if (dashboardData == null) dashboardData = new dashboard_view_model();

            return View("~/Views/Organiser/Dashboard.cshtml", dashboardData);

        }//end of Dashboard()

        //create event
        [HttpGet("CreateEvent")]
        public IActionResult CreateEvent()
        {//start of CreateEvent()

            if (HttpContext.Session.GetString("Role") != "Organiser") return RedirectToAction("Login", "Auth");
            return View("~/Views/Organiser/CreateEvent.cshtml");

        }//end of CreateEvent()

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
                        bool uploadSuccess = await _api_service.post_multipart_async($"events/{newEventId}/banner", model.BannerFile);

                        if (!uploadSuccess)
                        {
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

        //edit event
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

        //delete event
        [HttpPost("DeleteEvent/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser") return RedirectToAction("Login", "Auth");

            await _api_service.delete_async($"events/{id}");

            TempData["SuccessMessage"] = "Event deleted successfully.";
            return RedirectToAction("Dashboard");
        }

        //manage categories
        [HttpGet("Categories/{eventId}")]
        public async Task<IActionResult> ManageCategories(int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser") return RedirectToAction("Login", "Auth");

            var categories = await _api_service.get_async<List<category_view_model>>($"events/{eventId}/categories");
            ViewBag.EventId = eventId;
            return View("~/Views/Organiser/ManageCategories.cshtml", categories ?? new List<category_view_model>());
        }

        [HttpPost("AddCategory/{eventId}")]
        public async Task<IActionResult> AddCategory(int eventId, string categoryName, string catDescription, int? minAge, int? maxAge)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser") return RedirectToAction("Login", "Auth");

            string query = $"categories/{eventId}?categoryName={Uri.EscapeDataString(categoryName)}&catDescription={Uri.EscapeDataString(catDescription ?? "")}&minAge={minAge}&maxAge={maxAge}";
            await _api_service.post_async(query, new { });

            return RedirectToAction("ManageCategories", new { eventId });
        }

        [HttpPost("DeleteCategory/{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId, int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser") return RedirectToAction("Login", "Auth");

            await _api_service.delete_async($"categories/{categoryId}");
            return RedirectToAction("ManageCategories", new { eventId });
        }

        //view enrolments
        [HttpGet("Enrolments/{eventId}")]
        public async Task<IActionResult> EventEnrolments(int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser") return RedirectToAction("Login", "Auth");

            var enrolments = await _api_service.get_async<List<enrolment_view_model>>($"events/{eventId}/enrolments");
            ViewBag.EventId = eventId;
            return View("~/Views/Organiser/EventEnrolments.cshtml", enrolments ?? new List<enrolment_view_model>());
        }

        //record results
        [HttpGet("RecordResult/{enrolmentId}")]
        public IActionResult RecordResult(int enrolmentId, int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser") return RedirectToAction("Login", "Auth");

            var model = new record_result_view_model
            {
                Enrolment_id = enrolmentId,
                Event_id = eventId,
                ParticipantName = $"Participant ID: {enrolmentId}"
            };
            return View("~/Views/Organiser/RecordResult.cshtml", model);
        }

        [HttpPost("RecordResult/{enrolmentId}")]
        public async Task<IActionResult> RecordResult(record_result_view_model model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser") return RedirectToAction("Login", "Auth");

            string query = $"results/{model.Event_id}?enrolmentId={model.Enrolment_id}&finishTime={Uri.EscapeDataString(model.FinishTime)}&overallPosition={model.OverallPosition}&totalFinishers={model.TotalFinishers}";

            await _api_service.post_async(query, new { });

            TempData["SuccessMessage"] = "Result recorded successfully!";
            return RedirectToAction("EventEnrolments", new { eventId = model.Event_id });
        }

        //view results
        [HttpGet("ViewResults/{eventId}")]
        public async Task<IActionResult> ViewResults(int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser") return RedirectToAction("Login", "Auth");

            var results = await _api_service.get_async<List<my_result_view_model>>($"events/{eventId}/results");

            ViewBag.EventId = eventId;
            return View("~/Views/Organiser/ViewResults.cshtml", results ?? new List<my_result_view_model>());
        }

    }//end of class

}//end of namespace