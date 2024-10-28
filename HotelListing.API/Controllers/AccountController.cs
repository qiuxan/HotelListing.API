﻿using HotelListing.API.Contracts;
using HotelListing.API.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace HotelListing.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAuthManager _authManager;
    private readonly ILogger<AccountController> _logger;
    public AccountController(IAuthManager authManager, ILogger<AccountController> logger)
    {
        _authManager = authManager;
        _logger = logger;
    }

    // POST api/account/register
    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] ApiUserDto apiUserDto)
    {
        _logger.LogInformation($"Registration attempt for {apiUserDto.Email}");

        var errors = await _authManager.Register(apiUserDto);

        try
        {
             if (errors.Any())
             {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
             }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wrong in the {nameof(Register)} -- User Registration attempt for {apiUserDto.Email}");
            return Problem($"Someting Went Wrong in the {nameof(Register)}.Please contact support", statusCode:500);
        }
       
    }

    // POST api/account/login
    [HttpPost]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        _logger.LogInformation($"Login attempt for {loginDto.Email}");

        try
        {
            var authResponse = await _authManager.Login(loginDto);

            if (authResponse is null)
            {
                return Unauthorized("Invalid user credentials");
            }
            return Ok(authResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wrong in the {nameof(Login)} -- User Login attempt for {loginDto.Email}");
            return Problem($"Someting Went Wrong in the {nameof(Login)}.Please contact support", statusCode:500);
        }

    }

    // POST api/account/refreshtoken
    [HttpPost]
    [Route("refreshtoken")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> refreshtoken([FromBody] AuthResponseDto request)
    {

        var authResponse = await _authManager.VerifyRefreshToken(request);

        if (authResponse is null)
        {
            return Unauthorized("Invalid user credentials");
        }
        return Ok(authResponse);
    }
}
