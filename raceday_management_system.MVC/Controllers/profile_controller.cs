using Microsoft.AspNetCore.Mvc;
using raceday_management_system.MVC.Models;
using raceday_management_system.MVC.Services;

namespace raceday_management_system.MVC.Controllers
{//start of namespace

    [Route("profile")]
    public class profile_controller : Controller
    {//start of class

        private readonly api_service _api_service;

        public profile_controller(api_service api_service)
        {//start of constructor
            _api_service = api_service;
        }//end of constructor

        [HttpGet]
        public async Task<IActionResult> Index()
        {//start of Index()
            // Any logged-in user can view their profile
            if (HttpContext.Session.GetString("User_id") == null) return RedirectToAction("Login", "Auth");

            var profile = await _api_service.get_async<profile_view_model>("users/profile");
            if (profile == null) return NotFound();

            return View("~/Views/Profile/Index.cshtml", profile);
        }//end of Index()

        [HttpPost("Update")]
        public async Task<IActionResult> Update(string firstname, string surname, string phoneNumber, DateOnly? dateOfBirth)
        {//start of Update()
            if (HttpContext.Session.GetString("User_id") == null) return RedirectToAction("Login", "Auth");

            string query = $"users/profile?firstname={Uri.EscapeDataString(firstname)}&surname={Uri.EscapeDataString(surname)}&phoneNumber={Uri.EscapeDataString(phoneNumber ?? "")}&dateOfBirth={dateOfBirth?.ToString("yyyy-MM-dd")}";

            await _api_service.put_async(query, new { });

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }//end of Update()

        [HttpPost("UploadPicture")]
        public async Task<IActionResult> UploadPicture(IFormFile file)
        {//start of UploadPicture()
            if (HttpContext.Session.GetString("User_id") == null) return RedirectToAction("Login", "Auth");

            // The API enforces that only Participants can upload pictures
            if (HttpContext.Session.GetString("Role") != "Participant")
            {
                TempData["ErrorMessage"] = "Only participants can upload profile pictures.";
                return RedirectToAction("Index");
            }

            if (file != null && file.Length > 0)
            {
                await _api_service.post_multipart_async("users/profile-picture", file);
                TempData["SuccessMessage"] = "Profile picture updated!";
            }

            return RedirectToAction("Index");
        }//end of UploadPicture()

    }//end of class

}//end of namespace