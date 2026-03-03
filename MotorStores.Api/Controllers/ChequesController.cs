// Controllers/ChequesController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorStores.Application.DTOs;
using MotorStores.Application.Features.Cheques.Commands;
using MotorStores.Application.Features.Cheques.Queries;

namespace MotorStores.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ChequesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChequesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ChequeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ChequeDto>>> GetAllCheques()
        => Ok(await _mediator.Send(new GetAllChequesQuery()));

    [HttpGet("due-this-month")]
    [ProducesResponseType(typeof(IEnumerable<ChequeReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ChequeReportDto>>> GetDueThisMonth()
        => Ok(await _mediator.Send(new GetDueThisMonthQuery()));

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ChequeReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChequeReportDto>> GetChequeById(string id)
    {
        var cheque = await _mediator.Send(new GetChequeByIdQuery { Id = id });
        return cheque == null ? NotFound($"Cheque with ID {id} not found.") : Ok(cheque);
    }

    [HttpGet("overdue")]
    [ProducesResponseType(typeof(IEnumerable<ChequeReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ChequeReportDto>>> GetOverdue()
        => Ok(await _mediator.Send(new GetOverdueChequesQuery()));

    [HttpGet("cleared")]
    [ProducesResponseType(typeof(IEnumerable<ChequeReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ChequeReportDto>>> GetCleared()
        => Ok(await _mediator.Send(new GetClearedChequesQuery()));

    [HttpPost]
    [ProducesResponseType(typeof(ChequeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ChequeDto>> CreateCheque([FromBody] ChequeDto dto)
    {
        try
        {
            var cheque = await _mediator.Send(new CreateChequeCommand { Cheque = dto });
            return CreatedAtAction(nameof(GetChequeById), new { id = cheque.ChequeId }, cheque);
        }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [HttpPatch("{chequeId}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateStatus(string chequeId, [FromBody] UpdateChequeStatusRequest request)
    {
        try
        {
            await _mediator.Send(new UpdateChequeStatusCommand
            {
                ChequeId = chequeId,
                NewStatus = request.NewStatus,
                User = request.User
            });
            return Ok(new { message = "Status updated successfully" });
        }
        catch (InvalidOperationException ex) { return NotFound(ex.Message); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [HttpPatch("status/bulk")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateStatusBulk([FromBody] BulkUpdateChequeStatusRequest request)
    {
        try
        {
            await _mediator.Send(new UpdateChequeStatusBulkCommand
            {
                ChequeIds = request.ChequeIds,
                NewStatus = request.NewStatus,
                User = request.User
            });
            return Ok(new { message = "Statuses updated successfully", count = request.ChequeIds.Count });
        }
        catch (InvalidOperationException ex) { return NotFound(ex.Message); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [HttpPatch("{chequeId}/verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> MarkAsVerified(string chequeId)
    {
        try
        {
            await _mediator.Send(new MarkChequeAsVerifiedCommand { ChequeId = chequeId });
            return Ok(new { message = "Cheque marked as verified" });
        }
        catch (InvalidOperationException ex) { return NotFound(ex.Message); }
    }

    [HttpPut("{chequeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCheque(string chequeId, [FromBody] UpdateChequeRequest request)
    {
        try
        {
            await _mediator.Send(new UpdateChequeCommand { ChequeId = chequeId, Request = request });
            return Ok(new { message = "Cheque updated successfully" });
        }
        catch (InvalidOperationException ex) { return NotFound(ex.Message); }
    }

    [HttpGet("summary/bank-account/{bankAccountId}")]
    [ProducesResponseType(typeof(IEnumerable<ChequeStatusSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ChequeStatusSummaryDto>>> GetStatusSummaryByBankAccount(int bankAccountId)
        => Ok(await _mediator.Send(new GetStatusSummaryByBankAccountQuery { BankAccountId = bankAccountId }));

    [HttpGet("summary/bank-account/time/{bankAccountId}")]
    [ProducesResponseType(typeof(IEnumerable<ChequeStatusSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ChequeStatusSummaryDto>>> GetStatusSummaryByBankAccountTime(
        int bankAccountId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
        => Ok(await _mediator.Send(new GetStatusSummaryByBankAccountTimeQuery
        {
            BankAccountId = bankAccountId,
            StartDate = startDate,
            EndDate = endDate
        }));

    [HttpDelete("{chequeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCheque(int chequeId)
    {
        try
        {
            await _mediator.Send(new DeleteChequeCommand { ChequeId = chequeId });
            return Ok(new { message = "Cheque deleted successfully" });
        }
        catch (InvalidOperationException ex) { return NotFound(ex.Message); }
    }

    [HttpGet("current-month-total/bank-account/{bankAccountId}")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    public async Task<ActionResult<decimal>> GetCurrentMonthTotal(
    int bankAccountId,
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)
    => Ok(await _mediator.Send(new GetCurrentMonthTotalByAccountQuery
    {
        BankAccountId = bankAccountId,
        StartDate = startDate,
        EndDate = endDate
    }));
}

public class UpdateChequeStatusRequest
{
    public string NewStatus { get; set; } = null!;
    public string User { get; set; } = "System";
}