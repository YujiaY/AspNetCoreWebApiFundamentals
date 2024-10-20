using CityInfo_Dev.Entities;

namespace CityInfo_Dev.Services;

public interface ICityInfoRepository
{
    Task<IEnumerable<City>> GetCitiesAsync();
    Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
    Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);
    
    Task<bool> CityExistsAsync(int cityId);
    Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
    Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
    
    Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);
    
    void DeletePointOfInterest(PointOfInterest pointOfInterest);
    
    Task<bool> CityNameMatchesCityIdAsync(string? cityName, int cityId);
    
    Task<bool> SaveChangesAsync();
}