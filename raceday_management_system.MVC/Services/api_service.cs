using System.Text;
using System.Text.Json;

namespace raceday_management_system.MVC.Services
{//start of namespace
    public class api_service
    {//start of class
        private readonly HttpClient _http_client;
        private readonly IHttpContextAccessor _http_context_accessor;

        public api_service(HttpClient http_client, IHttpContextAccessor http_context_accessor)
        {//start of constructor
            _http_client = http_client;
            _http_context_accessor = http_context_accessor;
        }//end of constructor

        // Helper to get the API cookie from the MVC session
        private void attach_session_cookie()
        {//start of attach_session_cookie()
            var session_cookie = _http_context_accessor.HttpContext?.Session.GetString("ApiSessionCookie");

            if (!string.IsNullOrEmpty(session_cookie))
            {
                _http_client.DefaultRequestHeaders.Remove("Cookie");
                _http_client.DefaultRequestHeaders.Add("Cookie", session_cookie);
            }
        }//end of attach_session_cookie()

        // GET Request
        public async Task<T?> get_async<T>(string endpoint)
        {//start of get_async()
            attach_session_cookie();
            var response = await _http_client.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return default;
        }//end of get_async()

        // POST Request
        public async Task<(bool IsSuccess, string? ResponseContent)> post_async<T>(string endpoint, T data)
        {//start of post_async()
            attach_session_cookie();

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http_client.PostAsync(endpoint, content);
            var response_content = await response.Content.ReadAsStringAsync();

            // If the API sends back a session cookie during login, save it!
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                var session_cookie = cookies.FirstOrDefault(c => c.StartsWith(".AspNetCore.Session"));
                if (session_cookie != null)
                {
                    _http_context_accessor.HttpContext?.Session.SetString("ApiSessionCookie", session_cookie);
                }
            }

            return (response.IsSuccessStatusCode, response_content);
        }//end of post_async()

        // PUT Request
        public async Task<bool> put_async<T>(string endpoint, T data)
        {//start of put_async()
            attach_session_cookie();

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http_client.PutAsync(endpoint, content);
            return response.IsSuccessStatusCode;
        }//end of put_async()

        // DELETE Request
        public async Task<bool> delete_async(string endpoint)
        {//start of delete_async()
            attach_session_cookie();
            var response = await _http_client.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }//end of delete_async()

        // POST Multipart (For File Uploads)
        public async Task<bool> post_multipart_async(string endpoint, IFormFile file)
        {//start of post_multipart_async()
            attach_session_cookie();

            using (var content = new MultipartFormDataContent())
            {
                using (var stream = file.OpenReadStream())
                {
                    var file_content = new StreamContent(stream);
                    file_content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

                    content.Add(file_content, "file", file.FileName);

                    var response = await _http_client.PostAsync(endpoint, content);
                    return response.IsSuccessStatusCode;
                }
            }
        }//end of post_multipart_async()

    }//end of class

}//end of namespace