using Microsoft.AspNetCore.Mvc;
using raceday_management_system.MVC.Models;
using raceday_management_system.MVC.Services;
using System.Text.Json;

namespace raceday_management_system.MVC.Controllers
{//start of namespace

    [Route("Participant")]
    public class participant_controller : Controller
    {//start of class

        private readonly api_service _api_service;

        public participant_controller(api_service api_service)
        {//start of constructor
            _api_service = api_service;
        }//end of constructor

        [HttpGet("MyEvents")]
        public async Task<IActionResult> MyEvents()
        {//start of MyEvents()
            if (HttpContext.Session.GetString("Role") != "Participant") return RedirectToAction("Login", "Auth");

            // Fetch enrolments
            var enrolments = await _api_service.get_async<List<dynamic>>("enrolments/me");
            // Fetch all events to join names
            var allEvents = await _api_service.get_async<List<event_card_view_model>>("events");

            var model = new List<my_event_view_model>();

            if (enrolments != null && allEvents != null)
            {
                foreach (var enrol in enrolments)
                {
                    var json = JsonSerializer.Serialize(enrol);
                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        var root = doc.RootElement;
                        int eventId = root.GetProperty("event_id").GetInt32();
                        int categoryId = root.GetProperty("category_id").GetInt32(); // GET THE CATEGORY ID

                        var matchedEvent = allEvents.FirstOrDefault(e => e.Event_id == eventId);

                        // FETCH CATEGORIES TO GET THE REAL NAME
                        var categories = await _api_service.get_async<List<category_view_model>>($"events/{eventId}/categories");
                        var matchedCategory = categories?.FirstOrDefault(c => c.Category_id == categoryId);
                        string categoryName = matchedCategory?.Category_name ?? "Unknown Category";

                        model.Add(new my_event_view_model
                        {
                            Enrolment_id = root.GetProperty("enrolment_id").GetInt32(),
                            Event_id = eventId,
                            Event_name = matchedEvent?.Event_name ?? $"Event #{eventId}",
                            Event_date = matchedEvent?.Event_date ?? DateTime.MinValue,
                            Enr_status = root.GetProperty("enr_status").GetString() ?? "Pending",
                            Payment_status = root.TryGetProperty("payment_status", out var ps) ? ps.GetString() : "Pending",
                            Category_name = categoryName // USE THE REAL NAME HERE
                        });
                    }
                }
            }

            return View("~/Views/Participant/MyEvents.cshtml", model);
        }//end of MyEvents()

        [HttpGet("MyResults")]
        public async Task<IActionResult> MyResults()
        {//start of MyResults()
            if (HttpContext.Session.GetString("Role") != "Participant") return RedirectToAction("Login", "Auth");

            var results = await _api_service.get_async<List<dynamic>>("results/me");
            var allEvents = await _api_service.get_async<List<event_card_view_model>>("events");

            var model = new List<my_result_view_model>();

            if (results != null && allEvents != null)
            {
                foreach (var res in results)
                {
                    var json = JsonSerializer.Serialize(res);
                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        var root = doc.RootElement;
                        int eventId = root.GetProperty("event_id").GetInt32();
                        var matchedEvent = allEvents.FirstOrDefault(e => e.Event_id == eventId);

                        model.Add(new my_result_view_model
                        {
                            Result_id = root.GetProperty("result_id").GetInt32(),
                            Event_name = matchedEvent?.Event_name ?? $"Event #{eventId}",
                            Event_date = matchedEvent?.Event_date ?? DateTime.MinValue,
                            Finish_time = root.GetProperty("finish_time").ToString(),
                            Overall_position = root.GetProperty("overall_position").GetInt32(),
                            Total_finishers = root.GetProperty("total_finishers").GetInt32()
                        });
                    }
                }
            }

            return View("~/Views/Participant/MyResults.cshtml", model);
        }//end of MyResults()

    }//end of class

}//end of namespace