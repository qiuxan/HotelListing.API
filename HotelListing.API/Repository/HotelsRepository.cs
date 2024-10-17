using HotelListing.API.Contracts;
using HotelListing.API.Data;

namespace HotelListing.API.Repository;

public class HotelsRepository : GenericRepository<Hotel>, IHotelsRebpository
{
    public HotelsRepository(HotelListingDbContext context) : base(context)
    {
    }
}
