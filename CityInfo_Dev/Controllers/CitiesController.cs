#region

using AutoMapper;
using CityInfo_Dev.Entities;
using CityInfo_Dev.Models;
using CityInfo_Dev.Services;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace CityInfo_Dev.Controllers;

[ApiController]
// [controller] is a token that will be replaced by the name of the controller, in this case, Cities
// [Route("api/[controller]")]
[Route("api/cities")]
public class CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper) : ControllerBase // Controller
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities()
    {
        var cityEntities = await cityInfoRepository.GetCitiesAsync();
        
        // var dtos = cityEntities.Select(e => new CityWithoutPointsOfInterestDto()
        // {
        //     Id = e.Id,
        //     Name = e.Name,
        //     Description = e.Description
        // });
        // return Ok(dtos);
        return Ok(mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
    }
    
    [HttpGet("{id}")]
    public ActionResult<City> GetCity(int id)
    {
        // City? cityToReturn = await cityInfoRepository.GetCityAsync(id, false);
        //
        // if (cityToReturn == null)
        // {
        //     return NotFound();
        // }
        //
        // return Ok(cityToReturn);
        return Ok();
    }
}