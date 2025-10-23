using MediatR;
using MotorStores.Application.DTOs;

namespace MotorStores.Application.Features.Vendors.Queries;

public record GetVendorByIdQuery : IRequest<VendorDto?>
{
    public int Id { get; init; }
}

public record GetVendorByCodeQuery : IRequest<VendorDto?>
{
    public string VendorCode { get; init; } = string.Empty;
}

public record GetAllVendorsQuery : IRequest<IEnumerable<VendorListDto>>
{
}

public record GetActiveVendorsQuery : IRequest<IEnumerable<VendorListDto>>
{
}

public record SearchVendorsQuery : IRequest<PaginatedVendorResponse>
{
    public VendorSearchDto SearchCriteria { get; init; } = null!;
}
