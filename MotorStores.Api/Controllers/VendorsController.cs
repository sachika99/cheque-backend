using MediatR;
using Microsoft.AspNetCore.Mvc;
using MotorStores.Application.DTOs;
using MotorStores.Application.Features.Vendors.Commands;
using MotorStores.Application.Features.Vendors.Queries;

namespace MotorStores.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VendorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public VendorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VendorListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VendorListDto>>> GetAllVendors()
    {
        var vendors = await _mediator.Send(new GetAllVendorsQuery());
        return Ok(vendors);
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<VendorListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VendorListDto>>> GetActiveVendors()
    {
        var vendors = await _mediator.Send(new GetActiveVendorsQuery());
        return Ok(vendors);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VendorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorDto>> GetVendorById(int id)
    {
        var vendor = await _mediator.Send(new GetVendorByIdQuery { Id = id });
        if (vendor == null)
        {
            return NotFound($"Vendor with ID {id} not found.");
        }
        return Ok(vendor);
    }

    [HttpGet("code/{vendorCode}")]
    [ProducesResponseType(typeof(VendorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorDto>> GetVendorByCode(string vendorCode)
    {
        var vendor = await _mediator.Send(new GetVendorByCodeQuery { VendorCode = vendorCode });
        if (vendor == null)
        {
            return NotFound($"Vendor with code '{vendorCode}' not found.");
        }
        return Ok(vendor);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(PaginatedVendorResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedVendorResponse>> SearchVendors(
        [FromQuery] string? searchTerm = null,
        [FromQuery] Domain.Enums.VendorStatus? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var searchCriteria = new VendorSearchDto
        {
            SearchTerm = searchTerm,
            Status = status,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(new SearchVendorsQuery { SearchCriteria = searchCriteria });
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(VendorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VendorDto>> CreateVendor([FromBody] CreateVendorDto createVendorDto)
    {
        try
        {
            var vendor = await _mediator.Send(new CreateVendorCommand { Vendor = createVendorDto });
            return CreatedAtAction(nameof(GetVendorById), new { id = vendor.Id }, vendor);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(VendorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorDto>> UpdateVendor(int id, [FromBody] UpdateVendorDto updateVendorDto)
    {
        if (id != updateVendorDto.Id)
        {
            return BadRequest("ID mismatch between URL and request body.");
        }

        try
        {
            var vendor = await _mediator.Send(new UpdateVendorCommand { Vendor = updateVendorDto });
            return Ok(vendor);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteVendor(int id)
    {
        var result = await _mediator.Send(new DeleteVendorCommand { Id = id });
        if (!result)
        {
            return NotFound($"Vendor with ID {id} not found.");
        }
        return NoContent();
    }

    [HttpPatch("{id}/activate")]
    [ProducesResponseType(typeof(VendorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorDto>> ActivateVendor(int id)
    {
        try
        {
            var vendor = await _mediator.Send(new ActivateVendorCommand { Id = id });
            return Ok(vendor);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id}/deactivate")]
    [ProducesResponseType(typeof(VendorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorDto>> DeactivateVendor(int id)
    {
        try
        {
            var vendor = await _mediator.Send(new DeactivateVendorCommand { Id = id });
            return Ok(vendor);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
