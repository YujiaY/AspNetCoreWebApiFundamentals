#region

using CityInfo_Dev.Models;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace CityInfo_Dev.Controllers;

[ApiController]
// [controller] is a token that will be replaced by the name of the controller, in this case, Cities
// [Route("api/[controller]")]
[Route("api/cities")]
public class CitiesController : ControllerBase // Controller
{
    [HttpGet]
    public JsonResult GetCities()
    {
        return new JsonResult(CitiesDataStore.Current.Cities);
        // return new JsonResult(
        //     new List<object>
        //     {
        //         new { Id = 1, Name = "New York City" },
        //         new { Id = 2, Name = "Antwerp" }
        //     });
    }
    
    [HttpGet("{id}")]
    public JsonResult GetCity(int id)
    {
        return new JsonResult(CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id));
    }
}