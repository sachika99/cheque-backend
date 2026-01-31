using Microsoft.AspNetCore.Mvc;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;

namespace MotorStores.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        // Get all invoices
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InvoiceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAll()
        {
            var invoices = await _invoiceService.GetAllAsync();
            return Ok(invoices);
        }

        // Get invoice by ID
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvoiceDto>> GetById(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);

            if (invoice == null)
                return NotFound($"Invoice with ID {id} not found.");

            return Ok(invoice);
        }

        // Create invoice
        [HttpPost]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InvoiceDto>> Create([FromBody] InvoiceDto dto)
        {
            var invoice = await _invoiceService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
        }

        // Update invoice
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<InvoiceDto>> Update(int id, [FromBody] InvoiceDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            var updated = await _invoiceService.UpdateAsync(dto);
            return Ok(updated);
        }

        // Delete invoice
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _invoiceService.DeleteAsync(id);

            if (!deleted)
                return NotFound($"Invoice with ID {id} not found.");

            return NoContent();
        }
    }
}
