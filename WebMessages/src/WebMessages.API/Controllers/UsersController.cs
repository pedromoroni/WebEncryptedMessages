using Azure.Core;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebMessages.API.Hubs;
using WebMessages.Data;
using WebMessages.Models;
using WebMessages.Models.DTOs;
using WebMessages.Models.DTOs.UserDevice;
using WebMessages.Models.DTOs.Users;
using WebMessages.Models.Entities;
using WebMessages.Services.Implementations;
using WebMessages.Services.Interfaces;

namespace WebMessages.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IDeviceService _deviceService;

    private readonly IHubContext<OrderHub> _hubContext;


    public UsersController(IUserService userService, IDeviceService deviceService, IHubContext<OrderHub> hubContext) 
    {
        _userService = userService;
        _deviceService = deviceService;
        _hubContext = hubContext;
    }

    [HttpGet("getUserByDeviceId")]
    [EndpointSummary("Get User by DevicedId")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserByDeviceId([FromQuery] Guid deviceId)
    {
        try
        {
            var response = await _userService.GetUserByDeviceIdAsync(deviceId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost("login")]
    [EndpointSummary("Authenticate User")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AuthenticateUser([FromBody] UserDeviceRequest userCredentials, [FromQuery] int pageSize, [FromQuery] int pageNumber)
    {
        try
        {
            await _deviceService.RegisterDeviceAsync(userCredentials);

            QueryInfo queryInfo = new QueryInfo
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            var response = await _userService.GetUserAsync(userCredentials.User, queryInfo);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost("create")]
    [EndpointSummary("Register User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            await _userService.CreateUserAsync(request);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the user.");
        }
    }

    [HttpGet("search")]
    [EndpointSummary("Search Users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchUsers([FromQuery] string username)
    {
        // TO DO: adicionar query info 
        try
        {
            var response = await _userService.SearchUsernamesStartingWithAsync(username);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the user.");
        }
    }
}

// verificaçoes para impedir que o public key seja invalido

/*[HttpPost("notify/{orderId}")]
public async Task<IActionResult> Notify(string orderId)
{
    // Envia para todos os clientes inscritos nesse orderId
    await _hubContext.Clients.Group(orderId).SendAsync("OrderStatusUpdated",
        new { Id = orderId, Status = "Hello World" });

    return Ok(new { Message = $"Notificação enviada para order {orderId}" });
}*/