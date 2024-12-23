﻿using HotelListing.API.Models.User;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Contracts;

public interface IAuthManager
{
    Task<IEnumerable<IdentityError>> Register(ApiUserDto userDTO);
    Task<AuthResponseDto>Login(LoginDto userDTO);

    Task<string> CreateRefreshToken();
    Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request);

}
