﻿namespace HotelListing.API.Models.Country.Hotel;

public class HotelDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Rating { get; set; }
    public string Address { get; set; }
    public int CountryId { get; set; }
}