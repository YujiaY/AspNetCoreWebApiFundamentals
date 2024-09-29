#region

using Microsoft.AspNetCore.Mvc;

#endregion

namespace CityInfo_Dev.Controllers;

[ApiController]
public class CitiesController : ControllerBase // Controller
{
    [HttpGet("api/cities")]
    public JsonResult GetCities()
    {
        return new JsonResult(
            new List<object>
            {
                new { Id = 1, Name = "New York City" },
                new { Id = 2, Name = "Antwerp" }
            });
    }
}