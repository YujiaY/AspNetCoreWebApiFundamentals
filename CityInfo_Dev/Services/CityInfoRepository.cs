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

    public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(
        string? name, string? searchQuery , int pageNumber, int pageSize)
    {
        // Commented out because we want to enforce the use of pagination
        // if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(searchQuery))
        // {
        //     return await GetCitiesAsync();
        // }
        
        // collection to start from
        IQueryable<City> collection = context.Cities.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            collection = collection.Where(c => c.Name == name);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            collection = collection.Where(c => c.Name.Contains(searchQuery)
                || c.Description != null && c.Description.Contains(searchQuery));
        }

        var totalItemCount = await collection.CountAsync();
        var metadata = new PaginationMetadata(
            totalItemCount,
            pageSize,
            pageNumber
        );

        var collectionToReturn = collection.OrderBy(c => c.Name)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize);
        
        return (collectionToReturn, metadata);
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

    public async Task<bool> CityNameMatchesCityIdAsync(string? cityName, int cityId)
    {
        return await context.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await context.SaveChangesAsync() >= 0);
    }

    
}