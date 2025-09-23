using Microsoft.AspNetCore.Mvc;
using WebMessages.Data;
using WebMessages.Models.DTOs;
using WebMessages.Models.DTOs.Messages;
using WebMessages.Models.DTOs.Users;
using WebMessages.Models.Entities;
using WebMessages.Models.Messages;
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
    public async Task<IActionResult> SendMessages([FromBody] SendMessagesRequest request)
    {
        try
        {
            QueryInfo queryInfo = new QueryInfo // ver se pode ser 0 ou se explode
            {
                PageNumber = 1,
                PageSize = 1
            };

            //  verificar se o user é valido e as credenciais estao corretas 
            UserDto user = await _userService.GetUserAsync(request.UserCredentials, queryInfo);

            await _messageService.SendMessages(request, user);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving user.");
        }
    }
}
