using AutoMapper;
using CityInfo_Dev.Entities;
using CityInfo_Dev.Models;

namespace CityInfo_Dev.Profiles;

public class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<City, CityDto>();
        CreateMap<City, CityWithoutPointsOfInterestDto>();
    }
}