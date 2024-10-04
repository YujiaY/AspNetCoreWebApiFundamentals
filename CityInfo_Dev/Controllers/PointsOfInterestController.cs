using CityInfo_Dev.Models;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo_Dev.Controllers;

[Route("api/cities/{cityId}/pointsofinterest")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        CityDto? city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

        if (city == null) return NotFound();

        return Ok(city.PointsOfInterest);
    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = CitiesDataStore.Current.Cities
            .FirstOrDefault(c => c.Id == cityId);

        if (city == null) return NotFound();

        var pointsOfInterest = city.PointsOfInterest
            .FirstOrDefault(p => p.Id == pointOfInterestId);

        if (pointsOfInterest == null) return NotFound();

        return Ok(pointsOfInterest);
    }

    [HttpPost]
    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
        int cityId,
        PointOfInterestForCreationDto pointOfInterest)
    {
        CityDto? city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

        if (city == null) return NotFound();

        // demo purposes - to be improved
        var maxPointOfInterestId = CitiesDataStore.Current.Cities
            .SelectMany(c => c.PointsOfInterest)
            .Max(p => p.Id);

        var finalPointOfInterest = new PointOfInterestDto
        {
            Id = ++maxPointOfInterestId,
            Name = pointOfInterest.Name,
            Description = pointOfInterest.Description
        };
        
        city.PointsOfInterest.Add(finalPointOfInterest);

        return CreatedAtRoute("GetPointOfInterest",
            new
            {
                cityId = cityId,
                pointOfInterestId = maxPointOfInterestId
            },
            finalPointOfInterest);
    }
    
    [HttpPatch("{pointOfInterestId}")]
    public async Task<ActionResult> UpdatePointOfInterest(
        int cityId,
        int pointOfInterestId,
        JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        CityDto? city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

        if (city == null)
        {
            return NotFound();
        }
        
        var pointOfInterestFromStore = city.PointsOfInterest
            .FirstOrDefault(p => p.Id == pointOfInterestId);
        
        if (pointOfInterestFromStore == null)
        {
            return NotFound();
        }
        
        var pointOfInterestToPatch = new PointOfInterestForUpdateDto
        {
            Name = pointOfInterestFromStore.Name,
            Description = pointOfInterestFromStore.Description
        };
        
        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
        
        if(ModelState.IsValid == false)
        {
            var result = BadRequest(ModelState);
            return result;
        }

        if (!TryValidateModel(pointOfInterestToPatch))
        {
            var result = BadRequest(ModelState);
            return result;
        }
        
        pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
        pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;
        
        return NoContent();
    }
}