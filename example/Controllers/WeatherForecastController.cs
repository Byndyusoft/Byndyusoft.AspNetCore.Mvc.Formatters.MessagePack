using Byndyusoft.AspNetCore.Mvc.Formatters.MessagePack.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Byndyusoft.AspNetCore.Mvc.Formatters.MessagePack.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        [HttpGet]
        [FormatFilter]

        public IActionResult Get()
        {
            var rng = new Random();
            var forecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
            })
                .ToArray();

            return Ok(forecast);
        }
    }
}