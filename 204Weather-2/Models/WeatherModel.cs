using Microsoft.AspNetCore.Mvc;

namespace _204Weather_2.Models
{
    public class WeatherModel
    {

        public WeatherModel()
        {


        }


        public string? City { get; set; }
        [BindProperty]
        public string? ZipCode { get; set; }


    }


}