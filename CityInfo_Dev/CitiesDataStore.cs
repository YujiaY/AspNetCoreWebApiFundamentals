#region

using CityInfo_Dev.Models;

#endregion

namespace CityInfo_Dev;

public class CitiesDataStore
{
    public List<CityDto> Cities { get; set; }
    public static CitiesDataStore Current { get; } = new CitiesDataStore();

    public CitiesDataStore()
    {
        // init dummy data
        Cities = new List<CityDto>()
        {
            new()
            {
                Id = 1,
                Name = "New York City",
                Description = "The one with that big park."
            },
            new()
            {
                Id = 2,
                Name = "Antwerp",
                Description = "The one with the cathedral that was never really finished."
            },
            new()
            {
                Id = 3,
                Name = "Paris",
                Description = "The one with that big tower."
            }
        };
    }
}