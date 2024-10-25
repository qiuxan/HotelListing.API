using AutoMapper;
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
    public AuthManager(IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration)
    {
        _mapper = mapper;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> Login(LoginDto loginDto)
    {
        bool isValidUser = false;
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        isValidUser = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (user is null || isValidUser == false)
        {
            return null;
        }

        var token = await GenerateToken(user);

        return new AuthResponseDto 
        {
            Token = token,
            UserId = user.Id
        };

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

    private async Task<string> GenerateToken(ApiUser apiUser)
    {
        var key = _configuration["JwtSettings:Key"];
     
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(apiUser);
        var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();
        var userClaims = await _userManager.GetClaimsAsync(apiUser);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, apiUser.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, apiUser.Email),
            new Claim("uid", apiUser.Id),
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
