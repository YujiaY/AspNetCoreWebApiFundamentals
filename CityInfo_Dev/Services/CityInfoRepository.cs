using CityInfo_Dev.DbContexts;
using CityInfo_Dev.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo_Dev.Services;

public class CityInfoRepository(CityInfoContext context) : ICityInfoRepository
{
    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        return await context.Cities
            .OrderBy(c => c.Name)
            .ToArrayAsync();
    }

    public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
    {
        if (includePointsOfInterest)
        {
            return await context.Cities.Include(c => c.PointsOfInterest)
                .Where(c => c.Id == cityId)
                .FirstOrDefaultAsync();
        }
        return await context.Cities
            .Where(c => c.Id == cityId)
            .FirstOrDefaultAsync();
    }

    public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
    {
        return await context.PointsOfInterest
                .Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
                .FirstOrDefaultAsync();
    }
    
    public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
    {
        return await context.PointsOfInterest
            .Where(p => p.CityId == cityId)
            .ToListAsync();
    }

    
}