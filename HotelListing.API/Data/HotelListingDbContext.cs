﻿using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data;

public class HotelListingDbContext : DbContext
{
    public HotelListingDbContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Country> Countries { get; set; }

    //seeding data
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Country>().HasData(
            new Country
            {
                Id = 1,
                Name = "Jamaica",
                ShortName = "JM"
            },
            new Country
            {
                Id = 2,
                Name = "Bahamas",
                ShortName = "BS"
            },
            new Country
            {
                Id = 3,
                Name = "Cayman Islands",
                ShortName = "CI"
            }
        );

        modelBuilder.Entity<Hotel>().HasData(
            new Hotel
            {
                Id = 1,
                Name = "Sandals Resort and Spa",
                Address = "Negril",
                CountryId = 1,
                Rating = 4.5
            },
            new Hotel
            {
                Id = 2,
                Name = "Grand Palladium",
                Address = "Nassau",
                CountryId = 2,
                Rating = 4
            },
            new Hotel
            {
                Id = 3,
                Name = "Comfort Suites",
                Address = "Seven Mile Beach",
                CountryId = 3,
                Rating = 4.3
            }
        );

 
    }
}
