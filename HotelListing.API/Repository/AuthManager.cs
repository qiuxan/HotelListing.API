﻿using AutoMapper;
using HotelListing.API.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing.API.Repository;

public class AuthManager : IAuthManager
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApiUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthManager> _logger;

    private ApiUser _user;

    private const string _refreshToken = "RefreshToken";
    private const string _loginProvider = "HetelListingApi";

    public AuthManager(IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration, ILogger<AuthManager> logger)
    {
        _mapper = mapper;
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> CreateRefreshToken()
    {
        await _userManager.RemoveAuthenticationTokenAsync(_user,_loginProvider, _refreshToken);
        var newRefreshToken = await _userManager.GenerateUserTokenAsync(_user, _loginProvider, _refreshToken);
        var resut = await _userManager.SetAuthenticationTokenAsync(_user, _loginProvider, _refreshToken, newRefreshToken);
        
        return newRefreshToken; 
    }

    public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);

        var username = tokenContent.Claims.ToList().FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;

        _user = await _userManager.FindByNameAsync(username);

        if(_user is null ||_user.Id != request.UserId)
        {
            return null;
        }
        var refreshTokenValid = await _userManager.VerifyUserTokenAsync(_user, _loginProvider, _refreshToken, request.RefreshToken);

        if (refreshTokenValid)
        {
            var token = await GenerateToken();

            return new AuthResponseDto
            {
                Token = token,
                UserId = _user.Id,
                RefreshToken = await CreateRefreshToken()
            };
        
        }

        await _userManager.UpdateSecurityStampAsync(_user);

        return null;
    }


    public async Task<AuthResponseDto> Login(LoginDto loginDto)
    {
        _logger.LogInformation($"Looking for user with email{loginDto.Email}");
        bool isValidUser = false;
        _user = await _userManager.FindByEmailAsync(loginDto.Email);
        isValidUser = await _userManager.CheckPasswordAsync(_user, loginDto.Password);

        if (_user is null || isValidUser == false)
        {
            _logger.LogWarning($"User not found with email {loginDto.Email}");
            return null;
        }

        var token = await GenerateToken();
        _logger.LogInformation($"Token generated for user with email {loginDto.Email} | tokem:{token}");
        return new AuthResponseDto 
        {
            Token = token,
            UserId = _user.Id,
            RefreshToken = await CreateRefreshToken()
        };

    }

    public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDTO)
    {
        _user = _mapper.Map<ApiUser>(userDTO);
        _user.UserName = userDTO.Email;
        
        var result = await _userManager.CreateAsync(_user, userDTO.Password);

        if (result.Succeeded) 
        {
            await _userManager.AddToRoleAsync(_user, "User");
        }
        return result.Errors;
    }

    private async Task<string> GenerateToken()
    {
        var key = _configuration["JwtSettings:Key"];
     
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(_user);
        var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();
        var userClaims = await _userManager.GetClaimsAsync(_user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, _user.Email),
            new Claim("uid", _user.Id),
        }
        .Union(userClaims).Union(roleClaims);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
