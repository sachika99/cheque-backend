using MediatR;
using MotorStores.Application.DTOs;

namespace MotorStores.Application.Features.Vendors.Commands;

public record CreateVendorCommand : IRequest<VendorDto>
{
    public CreateVendorDto Vendor { get; init; } = null!;
}

public record UpdateVendorCommand : IRequest<VendorDto>
{
    public UpdateVendorDto Vendor { get; init; } = null!;
}

public record DeleteVendorCommand : IRequest<bool>
{
    public int Id { get; init; }
}

public record ActivateVendorCommand : IRequest<VendorDto>
{
    public int Id { get; init; }
}

public record DeactivateVendorCommand : IRequest<VendorDto>
{
    public int Id { get; init; }
}
