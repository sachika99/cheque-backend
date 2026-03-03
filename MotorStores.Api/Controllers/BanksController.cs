// Controllers/BanksController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorStores.Application.DTOs;
using MotorStores.Application.Features.Banks.Commands;
using MotorStores.Application.Features.Banks.Queries;

namespace MotorStores.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class BanksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BanksController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BankDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BankDto>>> GetAllBanks()
    {
        try
        {
            var banks = await _mediator.Send(new GetAllBanksQuery());
            return Ok(banks);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BankDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BankDto>> GetBankById(int id)
    {
        try
        {
            var bank = await _mediator.Send(new GetBankByIdQuery { Id = id });
            return bank == null ? NotFound(new { message = $"Bank with ID {id} not found." }) : Ok(bank);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("account")]
    [ProducesResponseType(typeof(BankDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BankDto>> CreateBankWithAccounts([FromBody] BankWithAccountsDto dto)
    {
        try
        {
            var bank = await _mediator.Send(new CreateBankWithAccountsCommand { BankWithAccounts = dto });
            return CreatedAtAction(nameof(GetBankById), new { id = bank.Id }, bank);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(BankDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BankDto>> CreateBank([FromBody] BankDto dto)
    {
        try
        {
            var bank = await _mediator.Send(new CreateBankCommand { Bank = dto });
            return CreatedAtAction(nameof(GetBankById), new { id = bank.Id }, bank);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(BankDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BankDto>> UpdateBank(int id, [FromBody] BankDto dto)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID mismatch between URL and request body." });

        try
        {
            var bank = await _mediator.Send(new UpdateBankCommand { Bank = dto });
            return Ok(bank);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteBank(int id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteBankCommand { Id = id });
            return result ? NoContent() : NotFound(new { message = $"Bank with ID {id} not found." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}