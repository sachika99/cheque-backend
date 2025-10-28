using MotorStores.Domain.Enums;

namespace MotorStores.Application.DTOs;

public record CreateVendorDto
{
    public string? VendorCode { get; init; }  
    public string VendorName { get; init; } = string.Empty;
    public string? VendorAddress { get; init; }
    public string? VendorPhoneNo { get; init; }
    public string? VendorEmail { get; init; }
    public string? BankName { get; init; }
    public string? AccountNumber { get; init; }
    public int? CrediPeriodDays { get; init; }
    public string? Notes { get; init; }
    public string? ContactPerson { get; init; }
}

public record UpdateVendorDto
{
    public int Id { get; init; }
    public string VendorCode { get; init; } = string.Empty;
    public string VendorName { get; init; } = string.Empty;
    public string? VendorAddress { get; init; }
    public string? VendorPhoneNo { get; init; }
    public string? VendorEmail { get; init; }
    public string? BankName { get; init; }
    public string? AccountNumber { get; init; }
    public int? CrediPeriodDays { get; init; }
    public VendorStatus Status { get; init; }
    public string? Notes { get; init; }
    public string? ContactPerson { get; init; }
}

public record VendorDto
{
    public int Id { get; init; }
    public string VendorCode { get; init; } = string.Empty;
    public string VendorName { get; init; } = string.Empty;
    public string? VendorAddress { get; init; }
    public string? VendorPhoneNo { get; init; }
    public string? VendorEmail { get; init; }
    public string? BankName { get; init; }
    public string? AccountNumber { get; init; }
    public int? CrediPeriodDays { get; init; }
    public VendorStatus Status { get; init; }
    public string? Notes { get; init; }
    public string? ContactPerson { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public string? UpdatedBy { get; init; }
    public bool CanReceivePayments { get; init; }
}

public record VendorListDto
{
    public int Id { get; init; }
    public string VendorCode { get; init; } = string.Empty;
    public string VendorName { get; init; } = string.Empty;
    public string? VendorPhoneNo { get; init; }
    public string? VendorEmail { get; init; }
    public VendorStatus Status { get; init; }
    public string StatusDisplayName { get; init; } = string.Empty;
    public bool CanReceivePayments { get; init; }
}

public record VendorSearchDto
{
    public string? SearchTerm { get; init; }
    public VendorStatus? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public record PaginatedVendorResponse
{
    public IEnumerable<VendorListDto> Vendors { get; init; } = Enumerable.Empty<VendorListDto>();
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage { get; init; }
    public bool HasNextPage { get; init; }
}
