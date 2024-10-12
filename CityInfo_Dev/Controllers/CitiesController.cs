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
        
        return Ok(mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
        // var dtos = cityEntities.Select(e => new CityWithoutPointsOfInterestDto()
        // {
        //     Id = e.Id,
        //     Name = e.Name,
        //     Description = e.Description
        // });
        // return Ok(dtos);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CityDto>> GetCity(int id, bool includePointsOfInterest = false)
    {
        var cityEntity = await cityInfoRepository.GetCityAsync(id, includePointsOfInterest);

        if (includePointsOfInterest)
        {
            return Ok(mapper.Map<CityDto>(cityEntity));    
        }
        else
        {
            return Ok(mapper.Map<CityWithoutPointsOfInterestDto>(cityEntity));
        }
        
        // City? cityToReturn = await cityInfoRepository.GetCityAsync(id, false);
        //
        // if (cityToReturn == null)
        // {
        //     return NotFound();
        // }
        //
        // return Ok(cityToReturn);
    }
}