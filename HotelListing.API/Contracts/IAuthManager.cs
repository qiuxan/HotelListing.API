namespace HotelListing.API.Contracts;

public interface IAuthManager
{
   // Task<bool> ValidateUser(ApiUserDTO userDTO);
    Task<string> CreateToken();

}
