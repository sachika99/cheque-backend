using MediatR;
using Microsoft.AspNetCore.Mvc;
using MotorStores.Application.DTOs;
using MotorStores.Application.Features.Vendors.Queries;
using MotorStores.Application.Interfaces;
using MotorStores.Infrastructure.Services;

namespace MotorStores.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ChequesController : ControllerBase
    {
        private readonly IChequeService _chequeService;
        private readonly IMediator _mediator;

        public ChequesController(IChequeService chequeService, IMediator mediator)
        {
            _chequeService = chequeService;
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ChequeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ChequeDto>>> GetAllCheques()
        {
            var vendors = await _mediator.Send(new GetAllChequesQuery());
            return Ok(vendors);
        }

        [HttpGet("due-this-month")]
        [ProducesResponseType(typeof(IEnumerable<ChequeReportDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ChequeReportDto>>> GetDueThisMonth()
        {
            var cheques = await _chequeService.GetDueThisMonthAsync();
            return Ok(cheques);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ChequeReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ChequeReportDto>> GetChequeById(string id)
        {
            var cheque = await _chequeService.GetByIdAsync(id);

            if (cheque == null)
                return NotFound($"Cheque with ID {id} not found.");

            return Ok(cheque);
        }

        // Get overdue cheques
        [HttpGet("overdue")]
        [ProducesResponseType(typeof(IEnumerable<ChequeReportDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ChequeReportDto>>> GetOverdue()
        {
            var cheques = await _chequeService.GetOverdueChequesAsync();
            return Ok(cheques);
        }

        // Get cleared cheques
        [HttpGet("cleared")]
        [ProducesResponseType(typeof(IEnumerable<ChequeReportDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ChequeReportDto>>> GetCleared()
        {
            var cheques = await _chequeService.GetClearedChequesAsync();
            return Ok(cheques);
        }

        // Create a new cheque
        [HttpPost]
        [ProducesResponseType(typeof(ChequeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ChequeDto>> CreateCheque([FromBody] ChequeDto dto)
        {
            try
            {
                var cheque = await _chequeService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetAllCheques), new { id = cheque.ChequeId }, cheque);
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

        // Update cheque status
        [HttpPatch("{chequeId}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateStatus(
            string chequeId,
            [FromBody] UpdateChequeStatusRequest request)
        {
            try
            {
                await _chequeService.UpdateStatusAsync(chequeId, request.NewStatus, request.User);
                return Ok(new { message = "Status updated successfully" });
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

        // Mark cheque as verified
        [HttpPatch("{chequeId}/verify")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> MarkAsVerified(string chequeId)
        {
            try
            {
                await _chequeService.MarkAsVerifiedAsync(chequeId);
                return Ok(new { message = "Cheque marked as verified" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }

    // Request model for updating cheque status
    public class UpdateChequeStatusRequest
    {
        public string NewStatus { get; set; } = null!;
        public string User { get; set; } = "System";
    }
}
