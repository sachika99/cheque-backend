using AutoMapper;
using MotorStores.Application.DTOs;
using MotorStores.Domain.Entities;
using MotorStores.Domain.Enums;

namespace MotorStores.Application.Mappings;
 
public class ChequeMappingProfile : Profile
{
    //public ChequeMappingProfile()
    //{ 
    //    CreateMap<Vendor, VendorDto>()
    //        .ForMember(dest => dest.CanReceivePayments, opt => opt.MapFrom(src => src.CanReceivePayments()));

    //    CreateMap<Vendor, VendorListDto>()
    //        .ForMember(dest => dest.StatusDisplayName, opt => opt.MapFrom(src => GetStatusDisplayName(src.Status)))
    //        .ForMember(dest => dest.CanReceivePayments, opt => opt.MapFrom(src => src.CanReceivePayments()));
 
    //    CreateMap<CreateVendorDto, Vendor>()
    //        .ForMember(dest => dest.Id, opt => opt.Ignore())
    //        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => VendorStatus.Active))
    //        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
    //        .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
    //        .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
    //        .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore()) ;

    //    CreateMap<UpdateVendorDto, Vendor>()
    //        .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
    //        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
    //        .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
    //        .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore()) ;
    //}

    //private static string GetStatusDisplayName(VendorStatus status)
    //{
    //    return status switch
    //    {
    //        VendorStatus.Active => "Active",
    //        VendorStatus.Inactive => "Inactive",
    //        VendorStatus.Suspended => "Suspended",
    //        VendorStatus.Blacklisted => "Blacklisted",
    //        _ => "Unknown"
    //    };
    //}
}
 
public static class ChequeMapper
{
    private static readonly IMapper _mapper;

    //static ChequeMapper()
    //{
    //    var config = new MapperConfiguration(cfg =>
    //    {
    //        cfg.AddProfile<VendorMappingProfile>();
    //    });
    //    _mapper = config.CreateMapper();
    //}

    //public static VendorDto ToDto(Vendor vendor)
    //{
    //    return new VendorDto
    //    {
    //        Id = vendor.Id,
    //        VendorCode = vendor.VendorCode,
    //        VendorName = vendor.VendorName,
    //        VendorAddress = vendor.VendorAddress,
    //        VendorPhoneNo = vendor.VendorPhoneNo,
    //        VendorEmail = vendor.VendorEmail,
    //        BankName = vendor.BankName,
    //        AccountNumber = vendor.AccountNumber,
    //        CrediPeriodDays = vendor.CrediPeriodDays,
    //        Status = vendor.Status,
    //        Notes = vendor.Notes,
    //        ContactPerson = vendor.ContactPerson,
    //        CreatedAt = vendor.CreatedAt,
    //        UpdatedAt = vendor.UpdatedAt,
    //        CreatedBy = vendor.CreatedBy,
    //        UpdatedBy = vendor.UpdatedBy,
    //        CanReceivePayments = vendor.CanReceivePayments()
    //    };
    //}
    public static  ChequeDto MapToReportDto(Cheque cheque)
    {
        return new ChequeDto
        {
            Id = cheque.Id,
            ChequeId = cheque.ChequeId,
            SupplierId = cheque.VendorId,
            SupplierName = cheque.Vendor?.VendorName ?? "Unknown",
            BankAccountId = cheque.BankAccountId,
            AccountNo = cheque.BankAccount?.AccountNo ?? "Unknown",
            ChequeBookId = cheque.ChequeBookId,
            InvoiceNo = cheque.InvoiceNo,
            InvoiceDate = cheque.InvoiceDate,
            InvoiceAmount = cheque.InvoiceAmount,
            ChequeNo = cheque.ChequeNo,
            ChequeDate = cheque.ChequeDate,
            DueDate = cheque.DueDate,
            ChequeAmount = cheque.ChequeAmount,
            ReceiptNo = cheque.ReceiptNo,
            PayeeName = cheque.PayeeName,
            Status = cheque.Status.ToString(),
            IsVerified = cheque.IsVerified,
            IsOverdue = cheque.IsOverdue
        };
    }
    //public static VendorListDto ToListDto(Vendor vendor)
    //{
    //    return new VendorListDto
    //    {
    //        Id = vendor.Id,
    //        VendorCode = vendor.VendorCode,
    //        VendorName = vendor.VendorName,
    //        VendorPhoneNo = vendor.VendorPhoneNo,
    //        VendorEmail = vendor.VendorEmail,
    //        Status = vendor.Status,
    //        StatusDisplayName = GetStatusDisplayName(vendor.Status),
    //        CanReceivePayments = vendor.CanReceivePayments()
    //    };
    //}

    //private static string GetStatusDisplayName(VendorStatus status)
    //{
    //    return status switch
    //    {
    //        VendorStatus.Active => "Active",
    //        VendorStatus.Inactive => "Inactive",
    //        VendorStatus.Suspended => "Suspended",
    //        VendorStatus.Blacklisted => "Blacklisted",
    //        _ => "Unknown"
    //    };
    //}
}
