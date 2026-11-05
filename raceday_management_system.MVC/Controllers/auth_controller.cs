using Microsoft.AspNetCore.Mvc;
using raceday_management_system.MVC.Models;
using raceday_management_system.MVC.Services;
using System.Text.Json;

namespace raceday_management_system.MVC.Controllers
{//start of namespace
    [Route("Auth")] // This fixes the 404 URL error when clicking the nav links
    public class auth_controller : Controller
    {//start of class
        private readonly api_service _api_service;

        public auth_controller(api_service api_service)
        {//start of constructor
            _api_service = api_service;
        }//end of constructor

        [HttpGet("Login")]
        public IActionResult Login()
        {
            // Explicitly point to the Auth folder
            return View("~/Views/Auth/Login.cshtml");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(login_view_model model)
        {
            if (!ModelState.IsValid) return View("~/Views/Auth/Login.cshtml", model);

            //API expects query parameters for login
            string endpoint = $"authenticator/login?email={Uri.EscapeDataString(model.Email)}&password={Uri.EscapeDataString(model.Password)}";

            var result = await _api_service.post_async(endpoint, new { });

            if (result.IsSuccess && result.ResponseContent != null)
            {
                using (JsonDocument doc = JsonDocument.Parse(result.ResponseContent))
                {
                    var role = doc.RootElement.GetProperty("role").GetString();
                    var userId = doc.RootElement.GetProperty("userId").GetInt32();

                    //Save to MVC session
                    HttpContext.Session.SetInt32("User_id", userId);
                    HttpContext.Session.SetString("Role", role ?? "Participant");
                    HttpContext.Session.SetString("Email", model.Email);

                    // Fetch the profile to get the First Name and Profile Picture
                    var profile = await _api_service.get_async<profile_view_model>("users/profile");
                    if (profile != null)
                    {
                        HttpContext.Session.SetString("Firstname", profile.Firstname ?? "");

                        // Store the picture URL, fallback to default if empty
                        string picUrl = string.IsNullOrEmpty(profile.Profile_picture_url)
                                        ? "/images/default-profile.png"
                                        : profile.Profile_picture_url;
                        HttpContext.Session.SetString("ProfilePicture", picUrl);
                    }

                    if (role == "Organiser")
                        return RedirectToAction("Dashboard", "Organiser");
                    else
                        return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View("~/Views/Auth/Login.cshtml", model);
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            // Explicitly point to the Auth folder
            return View("~/Views/Auth/Register.cshtml");
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(register_view_model model)
        {
            if (!ModelState.IsValid) return View("~/Views/Auth/Register.cshtml", model);

            string dateStr = model.DateOfBirth?.ToString("yyyy-MM-dd") ?? "";
            string phoneStr = model.PhoneNumber ?? "";

            // API expects query parameters for register
            string endpoint = $"authenticator/register?email={Uri.EscapeDataString(model.Email)}&password={Uri.EscapeDataString(model.Password)}&firstname={Uri.EscapeDataString(model.Firstname)}&surname={Uri.EscapeDataString(model.Surname)}&role={model.Role}&dateOfBirth={dateStr}&phoneNumber={phoneStr}";

            var result = await _api_service.post_async(endpoint, new { });

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Registration successful. Please log in.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, "Registration failed. Email might already exist.");
            return View("~/Views/Auth/Register.cshtml", model);
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }//end of class

}//end of namespace