using Microsoft.AspNetCore.Mvc;
using WebMessages.Data;
using WebMessages.Models.DTOs.Messages;
using WebMessages.Models.DTOs.Users;
using WebMessages.Services.Interfaces;

namespace WebMessages.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost("add")]
    [EndpointSummary("Add message to user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddMessage([FromBody] MessageRequest request)
    {
        try
        {
            await _messageService.AddMessage(request);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving user.");
        }
    }
}
