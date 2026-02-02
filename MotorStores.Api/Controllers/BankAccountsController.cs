using Microsoft.AspNetCore.Mvc;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;

namespace MotorStores.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BankAccountsController : ControllerBase
    {
        private readonly IBankAccountService _bankAccountService;

        public BankAccountsController(IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }

        // Get all bank accounts
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BankAccountDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BankAccountDto>>> GetAllAccounts()
        {
            var accounts = await _bankAccountService.GetAllAsync();
            return Ok(accounts);
        }

        // Get all accounts for a specific bank
        [HttpGet("bank/{bankId}")]
        [ProducesResponseType(typeof(IEnumerable<BankAccountDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BankAccountDto>>> GetAccountsByBank(int bankId)
        {
            var accounts = await _bankAccountService.GetByBankIdAsync(bankId);
            return Ok(accounts);
        }

        // Get bank account by ID
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BankAccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BankAccountDto>> GetAccountById(int id)
        {
            var account = await _bankAccountService.GetByIdAsync(id);

            if (account == null)
                return NotFound($"Bank account with ID {id} not found.");

            return Ok(account);
        }

        // Create a new bank account
        [HttpPost]
        [ProducesResponseType(typeof(BankAccountDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BankAccountDto>> CreateAccount([FromBody] BankAccountDto dto)
        {
            try
            {
                var account = await _bankAccountService.CreateAsync(dto);

                return CreatedAtAction(nameof(GetAccountById), new { id = account.Id }, account);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Update an existing bank account
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BankAccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BankAccountDto>> UpdateAccount(int id, [FromBody] BankAccountDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch between URL and request body.");

            try
            {
                var account = await _bankAccountService.UpdateAsync(dto);
                return Ok(account);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAccount(int id)
        {
            try
            {
                var result = await _bankAccountService.DeleteAsync(id);

                if (!result)
                    return NotFound($"Bank account with ID {id} not found.");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // Activate a bank account (deactivate others in same bank)
        [HttpPatch("{id}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActivateAccount(int id)
        {
            try
            {
                await _bankAccountService.ActivateAccountAsync(id);
                return Ok(new { message = "Account activated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
