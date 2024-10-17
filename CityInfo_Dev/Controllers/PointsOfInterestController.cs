using AutoMapper;
using CityInfo_Dev.Entities;
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
    private readonly IMailService _mailService;
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;
    
    private const int CityNotFound = 1000;
    private const int InvalidInput = 1001;

    public PointsOfInterestController(
        ILogger<PointsOfInterestController> logger,
        IMailService mailService,
        ICityInfoRepository cityInfoRepository,
        IMapper mapper
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ??
                            throw new ArgumentNullException(nameof(mailService));
        _cityInfoRepository = cityInfoRepository ??
                              throw new ArgumentNullException(nameof(cityInfoRepository));
        _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
        // if (_logger == null)
        // {
        // HttpContext is not available in the constructor
        //     _logger = HttpContext?.RequestServices?
        //         .GetService<ILogger<PointsOfInterestController>>();
        //               // ?? throw new InvalidOperationException();
        // }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
    {
        // throw new Exception("Test exception wahaha~");
        try
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation("City with id {CityId} was not found when accessing points of interest", cityId);
                return NotFound();
            }
            IEnumerable<PointOfInterest> pointsOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
            // if (city == null)
            // {
            //     if (_logger == null)
            //     {
            //         _logger = HttpContext.RequestServices
            //                       .GetService<ILogger<PointsOfInterestController>>()
            //                   ?? throw new InvalidOperationException();
            //     }
            //     _logger.LogInformation(new EventId(CityNotFound, "CityCouldNotBeFound"), 
            //         "City with id {CityId} wasn't found when accessing points of interest", 
            //         cityId);
            //     return NotFound();
            // }
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Exception while getting points of interest for city with id {CityId}", ex, cityId);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            _logger.LogInformation("City with id {CityId} was not found when accessing points of interest", cityId);
            return NotFound();
        }
        
        var pointsOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

        if (pointsOfInterest == null) return NotFound();

        return Ok(_mapper.Map<PointOfInterestDto>(pointsOfInterest));
    }

    [HttpPost]
    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
        int cityId,
        PointOfInterestForCreationDto pointOfInterest)
    {
        City? city = await _cityInfoRepository.GetCityAsync(cityId, true);
    
        if (city == null)
        {
            return NotFound();
        }
        
        var finalPointOfInterestEntity = _mapper.Map<PointOfInterest>(pointOfInterest);
        
        await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterestEntity);
        
        var createdPointOfInterest = _mapper.Map<PointOfInterestDto>(finalPointOfInterestEntity);
        
        return CreatedAtRoute("GetPointOfInterest",
            new
            {
                cityId = cityId,
                pointOfInterestId = createdPointOfInterest.Id
            },
            createdPointOfInterest);
    }
    //
    // [HttpPatch("{pointOfInterestId}")]
    // public ActionResult UpdatePointOfInterest(
    //     int cityId,
    //     int pointOfInterestId,
    //     JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    // {
    //     CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
    //
    //     if (city == null) return NotFound();
    //
    //     var pointOfInterestFromStore = city.PointsOfInterest
    //         .FirstOrDefault(p => p.Id == pointOfInterestId);
    //
    //     if (pointOfInterestFromStore == null) return NotFound();
    //
    //     var pointOfInterestToPatch = new PointOfInterestForUpdateDto
    //     {
    //         Name = pointOfInterestFromStore.Name,
    //         Description = pointOfInterestFromStore.Description
    //     };
    //
    //     patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
    //
    //     if (ModelState.IsValid == false)
    //     {
    //         var result = BadRequest(ModelState);
    //         return result;
    //     }
    //
    //     if (!TryValidateModel(pointOfInterestToPatch))
    //     {
    //         var result = BadRequest(ModelState);
    //         return result;
    //     }
    //
    //     pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
    //     pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;
    //
    //     return NoContent();
    // }
    //
    // [HttpDelete("{pointOfInterestId}")]
    // public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
    // {
    //     _mailService.Send("Point of interest deleted attempt.",
    //         $"Point of interest with id {pointOfInterestId} is being deleted.");
    //
    //     CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
    //
    //     if (city == null) return NotFound();
    //
    //     var pointOfInterestFromStore = city.PointsOfInterest
    //         .FirstOrDefault(p => p.Id == pointOfInterestId);
    //
    //     if (pointOfInterestFromStore == null) return NotFound();
    //     _mailService.Send("Point of interest deleted.",
    //         $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");
    //     city.PointsOfInterest.Remove(pointOfInterestFromStore);
    //
    //     return NoContent();
    // }
}