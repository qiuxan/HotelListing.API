﻿using HotelListing.API.Contracts;
using HotelListing.API.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAuthManager _authManager;
    public AccountController(IAuthManager authManager)
    {
        _authManager = authManager;
    }

    // POST api/account/register
    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] ApiUserDto apiUserDto)
    {
        var errors = await _authManager.Register(apiUserDto);

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

    // POST api/account/login
    [HttpPost]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {

        var authResponse = await _authManager.Login(loginDto);

        if (authResponse is null)
        {
            return Unauthorized("Invalid user credentials");
        }
        return Ok(authResponse);
    }
}
