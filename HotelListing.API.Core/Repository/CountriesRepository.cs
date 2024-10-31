using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.API.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Exceptions;
using HotelListing.API.Models.Country;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repository;

public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
{
    private readonly HotelListingDbContext _context;
    private readonly IMapper _mapper;
    public CountriesRepository(HotelListingDbContext context, IMapper mapper) : base(context,mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CountryDto> GetDetails(int id)
    {
        var country = await _context.Countries
            .Include(c => c.Hotels)
            .ProjectTo<CountryDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (country is null) { throw new NotFoundException(nameof(Country), id); }

        return country;
    }
}
