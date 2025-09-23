using Microsoft.AspNetCore.Mvc;
using WebMessages.Services.Implementations;
using WebMessages.Services.Interfaces;

namespace WebMessages.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet("getDevicesByUserId")]
    [EndpointSummary("Get Devices by UserId")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDeviceByUserId([FromQuery] Guid userId)
    {
        try
        {
            var response = await _deviceService.GetDevicesByUserIdAsync(userId);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the data.");
        }
    }

    [HttpGet("getDeviceByPublicKey")]
    [EndpointSummary("Get Devices by Public Key")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDeviceByPublicKey([FromQuery] byte[] publicKey)
    {
        try
        {
            var response = await _deviceService.GetDeviceByPublicKeyAsync(publicKey);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the data.");
        }
    }
}
