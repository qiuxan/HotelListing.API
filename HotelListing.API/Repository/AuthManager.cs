using AutoMapper;
using HotelListing.API.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Models.User;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Repository;

public class AuthManager : IAuthManager
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApiUser> _userManager;
    public AuthManager(IMapper mapper, UserManager<ApiUser> userManager)
    {
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDTO)
    {
        var user = _mapper.Map<ApiUser>(userDTO);
        user.UserName = userDTO.Email;
        
        var result = await _userManager.CreateAsync(user, userDTO.Password);

        if (result.Succeeded) 
        {
            await _userManager.AddToRoleAsync(user,"User");
        }
        return result.Errors;
    }
}
