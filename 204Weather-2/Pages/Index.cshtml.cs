using _204Weather_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Http.HttpClient;

namespace _204Weather_2.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        [BindProperty]
        public Main WeatherResponses { get; set; } = new Main();
        [BindProperty]
        public WeatherModel Weather { get; set; } = new WeatherModel();
        [BindProperty]
        public GeoResponse Geo { get; set; } = new GeoResponse();


        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void OnGet()
        {
            ViewData["ViewWeather"] = "name to start";
          
        }

        public object Get_configuration()
        {
            return _configuration;
        }

        public async Task<IActionResult> OnPostGetWeatherAsync()
        {
            string apiKey = Environment.GetEnvironmentVariable("apikey");
            using (HttpClient client = new HttpClient())
            {
                using var response1 = await client.GetAsync(requestUri: $"http://api.openweathermap.org/geo/1.0/zip?zip={Weather.ZipCode},us&appid={apiKey}");
                {
                    if (response1.IsSuccessStatusCode)
                    {
                        var locationContent = await response1.Content.ReadAsStringAsync();
                        GeoResponse ? location = JsonSerializer.Deserialize<GeoResponse>(locationContent);
                        Geo = location ?? new GeoResponse();
                        using var response = await client.GetAsync(requestUri: $"https://api.openweathermap.org/data/2.5/weather?lat={location.lat}&lon={location.lon}&appid={apiKey}");
                        if (response.IsSuccessStatusCode)
                        {

                            var content = await response.Content.ReadAsStringAsync();
                            WeatherResponse ? details = JsonSerializer.Deserialize<WeatherResponse>(content);
                            // Now you can work with the deserialized data in the 'details' object.
                            WeatherResponses = details?.main ?? new Main() { temp = 1 };
                            string tempF = (((WeatherResponses.temp - 273.15) *1.8) +32).ToString("0.0");

                            // Calling Azure Function
                            using var response2 = await client.GetAsync(requestUri: $"https://rob-weather-functions.azurewebsites.net/api/Function1?temp={tempF}");
                            {
                                if (response2.IsSuccessStatusCode)
                                {
                                    var functionDetails = response2.Content.ReadAsStringAsync();
                                    Debug.WriteLine(functionDetails.Result);
                                    Console.WriteLine(functionDetails.Result.ToString());
                                    ViewData["Warning"] = functionDetails.Result.ToString();
                                }
                            };

                            ViewData["ViewWeather"] = WeatherResponses.temp_min.ToString();
                            return Page();
                        }
                        else
                        {
                            // Handle the case where the API call was not successful.
                            return RedirectToPage();
                        }
                    }
                    else { return RedirectToPage(); }
                }
                
            }
            

        }


    }

    

       

   
}