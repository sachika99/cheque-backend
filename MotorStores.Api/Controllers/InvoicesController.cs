// Controllers/InvoicesController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorStores.Application.DTOs;
using MotorStores.Application.Features.Invoices.Commands;
using MotorStores.Application.Features.Invoices.Queries;

namespace MotorStores.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAll()
    {
        try
        {
            return Ok(await _mediator.Send(new GetAllInvoicesQuery()));
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InvoiceDto>> GetById(int id)
    {
        try
        {
            var invoice = await _mediator.Send(new GetInvoiceByIdQuery { Id = id });
            return invoice == null
                ? NotFound(new { message = $"Invoice with ID {id} not found." })
                : Ok(invoice);
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpPost]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InvoiceDto>> Create([FromBody] InvoiceDto dto)
    {
        try
        {
            var invoice = await _mediator.Send(new CreateInvoiceCommand { Invoice = dto });
            return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InvoiceDto>> Update(int id, [FromBody] InvoiceDto dto)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID mismatch." });

        try
        {
            var updated = await _mediator.Send(new UpdateInvoiceCommand { Invoice = dto });
            return Ok(updated);
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return NotFound(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _mediator.Send(new DeleteInvoiceCommand { Id = id });
            return deleted
                ? NoContent()
                : NotFound(new { message = $"Invoice with ID {id} not found." });
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }
}