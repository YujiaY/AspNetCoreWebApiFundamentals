using CityInfo_Dev.Models;
using CityInfo_Dev.Services;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo_Dev.Controllers;

[Route("api/cities/{cityId}/pointsofinterest")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    private ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _localMailService;
    private readonly CitiesDataStore _citiesDataStore;
    
    private const int CityNotFound = 1000;
    private const int InvalidInput = 1001;

    public PointsOfInterestController(
        ILogger<PointsOfInterestController> logger,
        IMailService mailService,
        CitiesDataStore citiesDataStore
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _localMailService = mailService ??
                            throw new ArgumentNullException(nameof(mailService));
        _citiesDataStore = citiesDataStore ??
                           throw new ArgumentNullException(nameof(citiesDataStore));
        // if (_logger == null)
        // {
        // HttpContext is not available in the constructor
        //     _logger = HttpContext?.RequestServices?
        //         .GetService<ILogger<PointsOfInterestController>>();
        //               // ?? throw new InvalidOperationException();
        // }
    }

    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        // throw new Exception("Test exception wahaha~");
        try
        {
            CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                if (_logger == null)
                {
                    _logger = HttpContext.RequestServices
                                  .GetService<ILogger<PointsOfInterestController>>()
                              ?? throw new InvalidOperationException();
                }
                _logger.LogInformation(new EventId(CityNotFound, "CityCouldNotBeFound"), 
                    "City with id {CityId} wasn't found when accessing points of interest", 
                    cityId);
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = _citiesDataStore.Cities
            .FirstOrDefault(c => c.Id == cityId);

        if (city == null) return NotFound();

        var pointsOfInterest = city.PointsOfInterest
            .FirstOrDefault(p => p.Id == pointOfInterestId);

        if (pointsOfInterest == null) return NotFound();

        return Ok(pointsOfInterest);
    }

    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(
        int cityId,
        PointOfInterestForCreationDto pointOfInterest)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

        if (city == null) return NotFound();

        // demo purposes - to be improved
        var maxPointOfInterestId = _citiesDataStore.Cities
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
    public ActionResult UpdatePointOfInterest(
        int cityId,
        int pointOfInterestId,
        JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

        if (city == null) return NotFound();

        var pointOfInterestFromStore = city.PointsOfInterest
            .FirstOrDefault(p => p.Id == pointOfInterestId);

        if (pointOfInterestFromStore == null) return NotFound();

        var pointOfInterestToPatch = new PointOfInterestForUpdateDto
        {
            Name = pointOfInterestFromStore.Name,
            Description = pointOfInterestFromStore.Description
        };

        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        if (ModelState.IsValid == false)
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

    [HttpDelete("{pointOfInterestId}")]
    public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

        if (city == null) return NotFound();

        var pointOfInterestFromStore = city.PointsOfInterest
            .FirstOrDefault(p => p.Id == pointOfInterestId);

        if (pointOfInterestFromStore == null) return NotFound();
        _localMailService.Send("Point of interest deleted.",
            $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");
        city.PointsOfInterest.Remove(pointOfInterestFromStore);

        return NoContent();
    }
}