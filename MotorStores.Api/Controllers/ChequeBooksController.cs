// Controllers/ChequeBooksController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorStores.Application.DTOs;
using MotorStores.Application.Features.ChequeBooks.Commands;
using MotorStores.Application.Features.ChequeBooks.Queries;

namespace MotorStores.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ChequeBooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChequeBooksController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ChequeBookDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ChequeBookDto>>> GetAllChequeBooks()
    {
        try
        {
            return Ok(await _mediator.Send(new GetAllChequeBooksQuery()));
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpGet("account/{bankAccountId}")]
    [ProducesResponseType(typeof(IEnumerable<ChequeBookDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ChequeBookDto>>> GetChequeBooksByAccount(int bankAccountId)
    {
        try
        {
            return Ok(await _mediator.Send(new GetChequeBooksByAccountQuery { BankAccountId = bankAccountId }));
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ChequeBookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChequeBookDto>> GetChequeBookById(int id)
    {
        try
        {
            var chequeBook = await _mediator.Send(new GetChequeBookByIdQuery { Id = id });
            return chequeBook == null
                ? NotFound(new { message = $"Cheque book with ID {id} not found." })
                : Ok(chequeBook);
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ChequeBookDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ChequeBookDto>> CreateChequeBook([FromBody] ChequeBookDto dto)
    {
        try
        {
            var chequeBook = await _mediator.Send(new CreateChequeBookCommand { ChequeBook = dto });
            return CreatedAtAction(nameof(GetChequeBookById), new { id = chequeBook.Id }, chequeBook);
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ChequeBookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChequeBookDto>> UpdateChequeBook(int id, [FromBody] ChequeBookDto dto)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID mismatch between URL and request body." });

        try
        {
            var chequeBook = await _mediator.Send(new UpdateChequeBookCommand { ChequeBook = dto });
            return Ok(chequeBook);
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteChequeBook(int id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteChequeBookCommand { Id = id });
            return result
                ? NoContent()
                : NotFound(new { message = $"Cheque book with ID {id} not found." });
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpPost("{id}/next-cheque")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> GetNextChequeNumber(int id)
    {
        try
        {
            var chequeNumber = await _mediator.Send(new GetNextChequeNumberCommand { ChequeBookId = id });
            return Ok(new { chequeNumber });
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }

    [HttpPatch("{id}/current-cheque")]
    [ProducesResponseType(typeof(ChequeBookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChequeBookDto>> UpdateCurrentChequeNo(int id, [FromBody] string currentChequeNo)
    {
        try
        {
            var result = await _mediator.Send(new UpdateCurrentChequeNoCommand
            {
                ChequeBookId = id,
                CurrentChequeNo = currentChequeNo
            });
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return NotFound(new { message = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
    }
}