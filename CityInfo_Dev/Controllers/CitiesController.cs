#region

using System.Text.Json;
using AutoMapper;
using CityInfo_Dev.Entities;
using CityInfo_Dev.Models;
using CityInfo_Dev.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace CityInfo_Dev.Controllers;

[ApiController]
// [controller] is a token that will be replaced by the name of the controller, in this case, Cities
// [Route("api/[controller]")]
[Authorize]
[Route("api/cities")]
public class CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper) : ControllerBase // Controller
{
    private const int MaxCitiesPageSize = 20;
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(
        [FromQuery(Name = "nameFilter")] string? name,
        string? searchQuery,
        int pageNumber = 1,
        int pageSize = 10
        )
    {
        if (pageSize > MaxCitiesPageSize)
        {
            pageSize = MaxCitiesPageSize;
        }
        
        var (cityEntities, metadata) = await cityInfoRepository
            .GetCitiesAsync(name, searchQuery, pageNumber, pageSize);
        
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
        
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