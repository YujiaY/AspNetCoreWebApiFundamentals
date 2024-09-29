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
    public ActionResult<IEnumerable<CityDto>> GetCities()
    {
        return Ok(CitiesDataStore.Current.Cities);
    }
    
    [HttpGet("{id}")]
    public ActionResult<CityDto> GetCity(int id)
    {
        CityDto? cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
        
        if (cityToReturn == null)
        {
            return NotFound();
        }
        
        return Ok(cityToReturn);
    }
}