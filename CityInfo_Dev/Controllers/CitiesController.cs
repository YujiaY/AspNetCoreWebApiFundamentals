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
    public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
    {
        var cityEntity = await cityInfoRepository.GetCityAsync(id, includePointsOfInterest);

        if (cityEntity == null)
        {
            return NotFound();
        }
        
        if (includePointsOfInterest)
        {
            CityDto? result1 = mapper.Map<CityDto>(cityEntity);
            return Ok(result1);    
        }
        else
        {
            CityWithoutPointsOfInterestDto? result2 = mapper.Map<CityWithoutPointsOfInterestDto>(cityEntity);
            return Ok(result2);
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