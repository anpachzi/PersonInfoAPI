using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace PersonInfoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public PersonController()
        {
            _httpClient = new HttpClient();
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // API endpoint that accepts the first name as a parameter
        [HttpGet("{firstName}")]
        public async Task<IActionResult> GetPersonInfo(string firstName)
        {
            // Run all API calls in parallel
            var ageTask = GetAgeInfo(firstName);
            var genderTask = GetGenderInfo(firstName);
            var nationalityTask = GetNationalityInfo(firstName);

            await Task.WhenAll(ageTask, genderTask, nationalityTask);

            var response = new
            {
                Name = firstName,
                Age = ageTask.Result?.Age?.ToString() ?? "Not available",
                Gender = genderTask.Result?.Gender ?? "Not available",
                Nationality = nationalityTask.Result?.Country?.FirstOrDefault()?.Country_Id ?? "Not available"
            };

            return Ok(response);
        }

        // API call to get age info
        private async Task<AgeInfo> GetAgeInfo(string firstName)
        {
            string url = $"https://api.agify.io?name={firstName}";
            var response = await _httpClient.GetStringAsync(url);
            return JsonSerializer.Deserialize<AgeInfo>(response, _jsonOptions);
        }

        // API call to get gender info
        private async Task<GenderInfo> GetGenderInfo(string firstName)
        {
            string url = $"https://api.genderize.io?name={firstName}";
            var response = await _httpClient.GetStringAsync(url);
            return JsonSerializer.Deserialize<GenderInfo>(response, _jsonOptions);
        }

        // API call to get nationality info
        private async Task<NationalityResponse> GetNationalityInfo(string firstName)
        {
            string url = $"https://api.nationalize.io?name={firstName}";
            var response = await _httpClient.GetStringAsync(url);
            return JsonSerializer.Deserialize<NationalityResponse>(response, _jsonOptions);
        }
    }

    // Model classes to map the JSON responses
    public class AgeInfo
    {
        public double? Age { get; set; }
    }

    public class GenderInfo
    {
        public string Gender { get; set; }
    }

    public class CountryInfo
    {
        public string Country_Id { get; set; }
    }

    public class NationalityResponse
    {
        public List<CountryInfo> Country { get; set; }
    }
}
