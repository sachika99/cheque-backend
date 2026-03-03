// Controllers/BankAccountsController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorStores.Application.DTOs;
using MotorStores.Application.Features.BankAccounts.Commands;
using MotorStores.Application.Features.BankAccounts.Queries;

namespace MotorStores.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class BankAccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BankAccountsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BankAccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BankAccountDto>>> GetAllAccounts()
    {
        try
        {
            return Ok(await _mediator.Send(new GetAllBankAccountsQuery()));
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpGet("bank/{bankId}")]
    [ProducesResponseType(typeof(IEnumerable<BankAccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BankAccountDto>>> GetAccountsByBank(int bankId)
    {
        try
        {
            return Ok(await _mediator.Send(new GetBankAccountsByBankIdQuery { BankId = bankId }));
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BankAccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BankAccountDto>> GetAccountById(int id)
    {
        try
        {
            var account = await _mediator.Send(new GetBankAccountByIdQuery { Id = id });
            return account == null
                ? NotFound(new { message = $"Bank account with ID {id} not found." })
                : Ok(account);
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpPost]
    [ProducesResponseType(typeof(BankAccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BankAccountDto>> CreateAccount([FromBody] BankAccountDto dto)
    {
        try
        {
            var account = await _mediator.Send(new CreateBankAccountCommand { BankAccount = dto });
            return CreatedAtAction(nameof(GetAccountById), new { id = account.Id }, account);
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(BankAccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BankAccountDto>> UpdateAccount(int id, [FromBody] BankAccountDto dto)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID mismatch between URL and request body." });

        try
        {
            var account = await _mediator.Send(new UpdateBankAccountCommand { BankAccount = dto });
            return Ok(account);
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAccount(int id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteBankAccountCommand { Id = id });
            return result
                ? NoContent()
                : NotFound(new { message = $"Bank account with ID {id} not found." });
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpPatch("{id}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAccount(int id)
    {
        try
        {
            await _mediator.Send(new ActivateBankAccountCommand { Id = id });
            return Ok(new { message = "Account activated successfully." });
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return NotFound(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }
}