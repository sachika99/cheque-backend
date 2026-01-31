using Microsoft.AspNetCore.Mvc;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;

namespace MotorStores.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ChequeBooksController : ControllerBase
    {
        private readonly IChequeBookService _chequeBookService;

        public ChequeBooksController(IChequeBookService chequeBookService)
        {
            _chequeBookService = chequeBookService;
        }

        // Get all cheque books
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ChequeBookDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ChequeBookDto>>> GetAllChequeBooks()
        {
            var chequeBooks = await _chequeBookService.GetAllAsync();
            return Ok(chequeBooks);
        }

        // Get all cheque books for a specific bank account
        [HttpGet("account/{bankAccountId}")]
        [ProducesResponseType(typeof(IEnumerable<ChequeBookDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ChequeBookDto>>> GetChequeBooksByAccount(int bankAccountId)
        {
            var chequeBooks = await _chequeBookService.GetByBankAccountIdAsync(bankAccountId);
            return Ok(chequeBooks);
        }

        // Get cheque book by ID
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ChequeBookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ChequeBookDto>> GetChequeBookById(int id)
        {
            var chequeBook = await _chequeBookService.GetByIdAsync(id);

            if (chequeBook == null)
                return NotFound($"Cheque book with ID {id} not found.");

            return Ok(chequeBook);
        }

        // Create a new cheque book
        [HttpPost]
        [ProducesResponseType(typeof(ChequeBookDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ChequeBookDto>> CreateChequeBook([FromBody] ChequeBookDto dto)
        {
            try
            {
                var chequeBook = await _chequeBookService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetChequeBookById), new { id = chequeBook.Id }, chequeBook);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Update an existing cheque book
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ChequeBookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ChequeBookDto>> UpdateChequeBook(int id, [FromBody] ChequeBookDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch between URL and request body.");

            try
            {
                var chequeBook = await _chequeBookService.UpdateAsync(dto);
                return Ok(chequeBook);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Delete a cheque book
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteChequeBook(int id)
        {
            try
            {
                var result = await _chequeBookService.DeleteAsync(id);

                if (!result)
                    return NotFound($"Cheque book with ID {id} not found.");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get next available cheque number from a cheque book
        [HttpPost("{id}/next-cheque")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> GetNextChequeNumber(int id)
        {
            try
            {
                var chequeNumber = await _chequeBookService.GetNextChequeNumberAsync(id);
                return Ok(new { chequeNumber });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPatch("{id}/current-cheque")]
        [ProducesResponseType(typeof(ChequeBookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ChequeBookDto>> UpdateCurrentChequeNo(int id,[FromBody] int currentChequeNo)
        {
            try
            {
                var result = await _chequeBookService.UpdateCurrentChequeNoAsync(id, currentChequeNo);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
