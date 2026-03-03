using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorStores.Application.DTOs;
using MotorStores.Application.Features.UserIds.Commands;
using MotorStores.Application.Features.UserIds.Queries;

namespace MotorStores.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class UserIdsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserIdsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ── GET ALL ───────────────────────────────────────────────────────────────
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserIdDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserIdDto>>> GetAllUserIds()
    {
        var userIds = await _mediator.Send(new GetAllUserIdsQuery());
        return Ok(userIds);
    }

    // ── GET BY INT ID ─────────────────────────────────────────────────────────
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserIdDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserIdDto>> GetById(int id)
    {
        var userId = await _mediator.Send(new GetUserByIdQuery { Id = id });
        if (userId == null)
            return NotFound($"User with ID {id} not found.");

        return Ok(userId);
    }

    // ── GET BY USER STRING ID ─────────────────────────────────────────────────
    [HttpGet("by-user/{userId}")]
    [ProducesResponseType(typeof(UserIdDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserIdDto>> GetByUserId(string userId)
    {
        var result = await _mediator.Send(new GetIdByUserQuery { UserId = userId });
        if (result == null)
            return NotFound($"User with UserId '{userId}' not found.");

        return Ok(result);
    }

    // ── CREATE ────────────────────────────────────────────────────────────────
    [HttpPost]
    [ProducesResponseType(typeof(UserIdDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserIdDto>> CreateUserId([FromBody] CreateUserIdCommand command)
    {
        try
        {
            var created = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // ── UPDATE ────────────────────────────────────────────────────────────────
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UserIdDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserIdDto>> UpdateUserId(int id, [FromBody] UpdateUserIdCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch between URL and request body.");

        try
        {
            var updated = await _mediator.Send(command);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // ── DELETE ────────────────────────────────────────────────────────────────
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUserId(int id)
    {
        var result = await _mediator.Send(new DeleteUserIdCommand { Id = id });
        if (!result)
            return NotFound($"User with ID {id} not found.");

        return NoContent();
    }
}