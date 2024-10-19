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

    public async Task<IEnumerable<City>> GetCitiesAsync(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await GetCitiesAsync();
        }

        name = name.Trim();
        return await context.Cities
            .Where(c => c.Name == name)
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

    public async Task<bool> CityExistsAsync(int cityId)
    {
        return await context.Cities.AnyAsync(c => c.Id == cityId);
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

    public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
    {
        var city = await GetCityAsync(cityId, false);
        if (city != null)
        {
            city.PointsOfInterest.Add(pointOfInterest);
        }
        
        await context.SaveChangesAsync();
    }

    public void DeletePointOfInterest(PointOfInterest pointOfInterest)
    {
        context.PointsOfInterest.Remove(pointOfInterest);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await context.SaveChangesAsync() >= 0);
    }

    
}