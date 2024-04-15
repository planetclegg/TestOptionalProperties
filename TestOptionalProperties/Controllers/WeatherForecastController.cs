using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TestOptionalProperties.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    ///{
    ///   "age": 21,
    ///   "nullableAge": null,
    ///   "userName": "some name"
    /// } 
    /// </remarks>
    /// <param name="testModel"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult<string> OkGo(TestModel testModel)
    {
        Debug.WriteLine(testModel.ToString());

        int age = 65;
        
        testModel.Age.SetValueIfExists(ref age);
        
        return Ok(testModel.ToString() + $"age == {age}");
    }

    // [HttpGet(Name = "GetWeatherForecast")]
    // public IEnumerable<WeatherForecast> Get()
    // {
    //     return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    //         {
    //             Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
    //             TemperatureC = Random.Shared.Next(-20, 55),
    //             Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    //         })
    //         .ToArray();
    // }
}