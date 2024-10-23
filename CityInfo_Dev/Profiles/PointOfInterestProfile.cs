using AutoMapper;
using CityInfo_Dev.Entities;
using CityInfo_Dev.Models;
using CityInfo_Dev.Models;

namespace CityInfo_Dev.Profiles;

public class PointOfInterestProfile : Profile
{
    public PointOfInterestProfile()
    {
        CreateMap<PointOfInterest, PointOfInterestDto>(); 
        CreateMap<PointOfInterestForCreationDto, PointOfInterest>(); 
        CreateMap<PointOfInterestForUpdateDto, PointOfInterest>(); 
        CreateMap<PointOfInterest, PointOfInterestForUpdateDto>(); 
    }
}