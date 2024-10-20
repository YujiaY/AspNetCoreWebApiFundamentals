using AutoMapper;
using CityInfo_Dev.Entities;
using CityInfo_Dev.Models;
using CityInfo_Dev.Services;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo_Dev.Controllers;

[Route("api/cities/{cityId}/pointsofinterest")]
[Authorize(Policy = "MuseBeFromBrisbane")]
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
            // Below not needed because of the Authorize policy
            // var userCityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;
            //
            // if (!(await _cityInfoRepository.CityNameMatchesCityIdAsync(userCityName, cityId)))
            // {
            //     return Forbid();
            // }
            
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
        PointOfInterestForCreationDto pointOfInterestForCreationDto)
    {
        City? city = await _cityInfoRepository.GetCityAsync(cityId, true);
    
        if (city == null)
        {
            return NotFound();
        }
        
        PointOfInterest? finalPointOfInterestEntity = _mapper.Map<PointOfInterest>(pointOfInterestForCreationDto);
        
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
    
    [HttpPatch("{pointOfInterestId}")]
    public async Task<ActionResult> UpdatePointOfInterest(
        int cityId,
        int pointOfInterestId,
        JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        City? city = await _cityInfoRepository.GetCityAsync(cityId, true);
    
        if (city == null) return NotFound();
    
        var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        
        if (pointOfInterestEntity == null) return NotFound();
    
        var pointOfInterestToPatch = _mapper
            .Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

    
        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
    
        if (ModelState.IsValid == false)
        {
            BadRequestObjectResult result = BadRequest(ModelState);
            return result;
        }
    
        if (!TryValidateModel(pointOfInterestToPatch))
        {
            BadRequestObjectResult result = BadRequest(ModelState);
            return result;
        }

        _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

        await _cityInfoRepository.SaveChangesAsync();
    
        return NoContent();
    }

    [HttpPut("{pointOfInterestId}")]
    public async Task<ActionResult> UpdatePointOfInterest(
        int cityId,
        int pointOfInterestId,
        PointOfInterestForUpdateDto pointOfInterestDto)
    {
        City? city = await _cityInfoRepository.GetCityAsync(cityId, true);
    
        if (city == null) return NotFound();
        
        var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
    
        if (pointOfInterestEntity == null) return NotFound();
    
        _mapper.Map(pointOfInterestDto, pointOfInterestEntity);

        await _cityInfoRepository.SaveChangesAsync();
    
        return NoContent();
    }
    
    [HttpDelete("{pointOfInterestId}")]
    public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        City? city = await _cityInfoRepository.GetCityAsync(cityId, true);
    
        if (city == null) return NotFound();
        
        var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
    
        if (pointOfInterestEntity == null) return NotFound();
        
        _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

        await _cityInfoRepository.SaveChangesAsync();
        
        _mailService.Send("Point of interest deleted.",
            $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");
    
        return NoContent();
    }
}