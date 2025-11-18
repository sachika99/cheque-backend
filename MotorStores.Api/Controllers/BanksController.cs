using Microsoft.AspNetCore.Mvc;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;

namespace MotorStores.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BanksController : ControllerBase
    {
        private readonly IBankService _bankService;

        public BanksController(IBankService bankService)
        {
            _bankService = bankService;
        }

        // Get all banks
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BankDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BankDto>>> GetAllBanks()
        {
            var banks = await _bankService.GetAllAsync();
            return Ok(banks);
        }

        // Get bank by ID
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BankDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BankDto>> GetBankById(int id)
        {
            var bank = await _bankService.GetByIdAsync(id);

            if (bank == null)
                return NotFound($"Bank with ID {id} not found.");

            return Ok(bank);
        }

        // Create a new bank
        [HttpPost]
        [ProducesResponseType(typeof(BankDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BankDto>> CreateBank([FromBody] BankDto dto)
        {
            try
            {
                var bank = await _bankService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetBankById), new { id = bank.Id }, bank);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Update an existing bank
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BankDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BankDto>> UpdateBank(int id, [FromBody] BankDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch between URL and request body.");

            try
            {
                var bank = await _bankService.UpdateAsync(dto);
                return Ok(bank);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Delete a bank
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteBank(int id)
        {
            try
            {
                var result = await _bankService.DeleteAsync(id);

                if (!result)
                    return NotFound($"Bank with ID {id} not found.");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
