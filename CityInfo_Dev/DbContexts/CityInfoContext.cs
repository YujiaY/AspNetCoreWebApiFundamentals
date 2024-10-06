using CityInfo_Dev.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo_Dev.DbContexts;

public class CityInfoContext(DbContextOptions<CityInfoContext> dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<City> Cities { get; set; }
    public DbSet<PointOfInterest> PointsOfInterest { get; set; }

    // Moved to Program.cs
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseSqlite(configuration.GetConnectionString("cityinfo_dev"));
    //     
    //     base.OnConfiguring(optionsBuilder);
    // }
}