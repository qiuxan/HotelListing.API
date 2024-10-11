﻿using HotelListing.API.Models.Country.Hotel;

namespace HotelListing.API.Models.Country;

public class CountryDto: BaseCountryDto
{
    public int Id { get; set; }
    public List<HotelDto> Hotels { get; set; }
}

