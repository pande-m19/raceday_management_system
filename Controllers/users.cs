using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using raceday_management_system.Database.Interfaces;
using raceday_management_system.Services;

namespace raceday_management_system.Controllers
{//start of namespace

    [Route("api/[controller]")]
    [ApiController]
    public class users : ControllerBase
    {//start of class

        private readonly Iusers_connection _db;
        private readonly blob_storage_service _blob;

        public users(Iusers_connection db, blob_storage_service blob)
        {//start of constructor

            _db = db;
            _blob = blob;

        }//end of constructor

        private int? get_logged_in_user_id()
        {//start of get_logged_in_user_id()

            return HttpContext.Session.GetInt32("User_id");

        }//end of get_logged_in_user_id()

        /// <summary>
        /// Returns the logged-in user's full profile information (excluding password hash).
        /// </summary>
        /// <returns>200 OK with profile data. 401 Unauthorized if not logged in. 404 Not Found if user not found.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("profile")]
        public IActionResult get_own_profile()
        {//start of get_own_profile()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            var user = _db.get_user_profile(user_id.Value);

            if (user == null)
            {//start of if

                return NotFound(new { message = "User not found." });

            }//end of if

            return Ok(new
            {
                user_id = user.User_id,
                email = user.Email,
                firstname = user.Firstname,
                surname = user.Surname,
                role = user.Role,
                profile_picture_url = user.Profile_picture_url,
                date_of_birth = user.Date_of_birth,
                phone_number = user.Phone_number,
                created_at = user.Created_at,
                updated_at = user.Updated_at,
                is_active = user.Is_active
            });

        }//end of get_own_profile()

        /// <summary>
        /// Updates the logged-in user's profile information.
        /// </summary>
        /// <param name="firstname">New first name.</param>
        /// <param name="surname">New surname.</param>
        /// <param name="phoneNumber">Optional new phone number.</param>
        /// <param name="dateOfBirth">Optional new date of birth.</param>
        /// <returns>200 OK on success. 400 Bad Request. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("profile")]
        public IActionResult update_own_profile(string firstname, string surname, string? phoneNumber = null, DateOnly? dateOfBirth = null)
        {//start of update_own_profile()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            string message = _db.update_user_profile(
                user_id.Value, firstname, surname, phoneNumber, dateOfBirth);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return Ok(new
            {
                message = "Profile updated successfully.",
                user_id = user_id.Value
            });

        }//end of update_own_profile()

        /// <summary>
        /// Uploads a profile picture to Azure Blob Storage and saves the URL to the database.
        /// </summary>
        /// <param name="file">The image file to upload.</param>
        /// <returns>200 OK on success. 403 Forbidden if not a Participant. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("profile-picture")]
        public async Task<IActionResult> set_profile_picture(IFormFile file)
        {//start of set_profile_picture()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            var role = HttpContext.Session.GetString("Role");

            if (role != "Participant")
            {//start of if

                return StatusCode(403, new { message = "Only participants can upload profile pictures." });

            }//end of if

            // 1. Upload to Azure
            string? image_url = await _blob.upload_file_async(file);

            if (image_url == null)
            {//start of if

                return BadRequest(new { message = "Failed to upload image to Azure." });

            }//end of if

            // 2. Save the URL to the database
            string message = _db.update_profile_picture(user_id.Value, image_url);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return Ok(new
            {
                message = "Profile picture updated successfully.",
                profilePictureUrl = image_url
            });

        }//end of set_profile_picture()

        /// <summary>
        /// Returns another user's public profile information (limited fields).
        /// </summary>
        /// <param name="userId">The user ID to look up.</param>
        /// <returns>200 OK with public info. 404 Not Found. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("{userId}/profile")]
        public IActionResult get_user_public_profile(int userId)
        {//start of get_user_public_profile()

            if (get_logged_in_user_id() == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            var user = _db.get_user_public_profile(userId);

            if (user == null)
            {//start of if

                return NotFound(new { message = "User not found." });

            }//end of if

            return Ok(new
            {
                user_id = user.User_id,
                firstname = user.Firstname,
                surname = user.Surname,
                role = user.Role,
                profile_picture_url = user.Profile_picture_url
            });

        }//end of get_user_public_profile()

    }//end of class

}//end of namespace