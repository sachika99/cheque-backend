using MediatR;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Application.Mappings;
using MotorStores.Domain.Entities;
using MotorStores.Domain.Enums;

namespace MotorStores.Application.Features.Vendors.Queries;

//public class GetVendorByIdQueryHandler : IRequestHandler<GetVendorByIdQuery, VendorDto?>
//{
//    private readonly IVendorRepository _vendorRepository;

//    public GetVendorByIdQueryHandler(IVendorRepository vendorRepository)
//    {
//        _vendorRepository = vendorRepository;
//    }

//    public async Task<VendorDto?> Handle(GetVendorByIdQuery request, CancellationToken cancellationToken)
//    {
//        var vendor = await _vendorRepository.GetByIdAsync(request.Id, cancellationToken);
//        return vendor != null ? VendorMapper.ToDto(vendor) : null;
//    }
//}

//public class GetVendorByCodeQueryHandler : IRequestHandler<GetVendorByCodeQuery, VendorDto?>
//{
//    private readonly IVendorRepository _vendorRepository;

//    public GetVendorByCodeQueryHandler(IVendorRepository vendorRepository)
//    {
//        _vendorRepository = vendorRepository;
//    }

//    public async Task<VendorDto?> Handle(GetVendorByCodeQuery request, CancellationToken cancellationToken)
//    {
//        var vendor = await _vendorRepository.GetByVendorCodeAsync(request.VendorCode, cancellationToken);
//        return vendor != null ? VendorMapper.ToDto(vendor) : null;
//    }
//}

public class GetAllChequesQueryHandler : IRequestHandler<GetAllChequesQuery, IEnumerable<ChequeDto>>
{
    private readonly IChequeRepository _chequeRepository;

    public GetAllChequesQueryHandler(IChequeRepository chequeRepository)
    {
        _chequeRepository = chequeRepository;
    }

    public async Task<IEnumerable<ChequeDto>> Handle(GetAllChequesQuery request, CancellationToken cancellationToken)
    {
        var vendors = await _chequeRepository.GetAllAsync(cancellationToken);
        return vendors.Select(ChequeMapper.MapToReportDto);
    }
}
public class GetAllChequesQueryHandlerById : IRequestHandler<GetAllChequesQueryById, ChequeDto>
{
    private readonly IChequeRepository _chequeRepository;

    public GetAllChequesQueryHandlerById(IChequeRepository chequeRepository)
    {
        _chequeRepository = chequeRepository;
    }

    public async Task<ChequeDto> Handle(GetAllChequesQueryById request, CancellationToken cancellationToken)
    {
        var vendors = await _chequeRepository.GetByIdAsync(request.Id, cancellationToken);
        return vendors != null ? ChequeMapper.MapToReportDto(vendors) : null;
    }
}

//public class GetActiveVendorsQueryHandler : IRequestHandler<GetActiveVendorsQuery, IEnumerable<VendorListDto>>
//{
//    private readonly IVendorRepository _vendorRepository;

//    public GetActiveVendorsQueryHandler(IVendorRepository vendorRepository)
//    {
//        _vendorRepository = vendorRepository;
//    }

//    public async Task<IEnumerable<VendorListDto>> Handle(GetActiveVendorsQuery request, CancellationToken cancellationToken)
//    {
//        var vendors = await _vendorRepository.GetActiveVendorsAsync(cancellationToken);
//        return vendors.Select(VendorMapper.ToListDto);
//    }
//}

//public class SearchVendorsQueryHandler : IRequestHandler<SearchVendorsQuery, PaginatedVendorResponse>
//{
//    private readonly IVendorRepository _vendorRepository;

//    public SearchVendorsQueryHandler(IVendorRepository vendorRepository)
//    {
//        _vendorRepository = vendorRepository;
//    }

//    public async Task<PaginatedVendorResponse> Handle(SearchVendorsQuery request, CancellationToken cancellationToken)
//    {
//        IEnumerable<Domain.Entities.Vendor> vendors;

//        if (!string.IsNullOrEmpty(request.SearchCriteria.SearchTerm))
//        {
//            vendors = await _vendorRepository.SearchByNameAsync(request.SearchCriteria.SearchTerm, cancellationToken);
//        }
//        else
//        {
//            vendors = await _vendorRepository.GetAllAsync(cancellationToken);
//        }

//        if (request.SearchCriteria.Status.HasValue)
//        {
//            vendors = vendors.Where(v => v.Status == request.SearchCriteria.Status.Value);
//        }

//        var totalCount = vendors.Count();
//        var totalPages = (int)Math.Ceiling((double)totalCount / request.SearchCriteria.PageSize);

//        var pagedVendors = vendors
//            .Skip((request.SearchCriteria.PageNumber - 1) * request.SearchCriteria.PageSize)
//            .Take(request.SearchCriteria.PageSize)
//            .Select(VendorMapper.ToListDto);

//        return new PaginatedVendorResponse
//        {
//            Vendors = pagedVendors,
//            TotalCount = totalCount,
//            PageNumber = request.SearchCriteria.PageNumber,
//            PageSize = request.SearchCriteria.PageSize,
//            TotalPages = totalPages,
//            HasPreviousPage = request.SearchCriteria.PageNumber > 1,
//            HasNextPage = request.SearchCriteria.PageNumber < totalPages
//        };
//    }
//}
