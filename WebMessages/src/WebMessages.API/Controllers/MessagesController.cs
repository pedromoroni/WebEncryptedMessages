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
    private readonly IUserService _userService;

    public MessagesController(IMessageService messageService, IUserService userService)
    {
        _messageService = messageService;
        _userService = userService;
    }

    [HttpPost("send")]
    [EndpointSummary("Send message to user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendMessages([FromBody] List<MessageRequest> request)
    {
        try
        {
            await _messageService.SendMessages(request);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving user.");
        }
    }
}
