using AutoMapper;
using MediatR;
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
    public static ChequeDto ToDto(Cheque cheque)
    {
        return new ChequeDto
        {
            Id = cheque.Id,
            ChequeId = cheque.ChequeId,
            VendorId = cheque.VendorId,
            ChequeBookId = cheque.ChequeBookId,
            BankAccountId = cheque.BankAccountId,
            InvoiceNo = cheque.InvoiceNo,
            InvoiceDate = cheque.InvoiceDate,
            InvoiceAmount = cheque.InvoiceAmount,
            ReceiptNo = cheque.ReceiptNo,
            ChequeNo = cheque.ChequeNo,
            ChequeDate = cheque.ChequeDate,
            DueDate = cheque.DueDate,
            ChequeAmount = cheque.ChequeAmount,
            PayeeName = cheque.PayeeName,
            Status = cheque.Status,
            IsVerified = cheque.IsVerified,
            CreatedAt = cheque.CreatedAt,
            Invoices = cheque.Invoices.Select(i => new InvoiceDto
            {
                Id = i.Id,
                InvoiceNo = i.InvoiceNo,
                InvoiceAmount = i.InvoiceAmount
            }).ToList()
        };
    }

    public static ChequeDto MapToDto(Cheque cheque)
    {
        return new ChequeDto
        {
            Id = cheque.Id,
            ChequeId = cheque.ChequeId,
            VendorId = cheque.VendorId,
            VendorName = cheque.Vendor?.VendorName ?? "Unknown",
            BankAccountId = cheque.BankAccountId,
            AccountNo = cheque.BankAccount?.AccountNo ?? "Unknown",
            ChequeBookId = cheque.ChequeBookId,
            ChequeNo = cheque.ChequeNo,
            InvoiceDate = cheque.InvoiceDate,
            ChequeDate = cheque.ChequeDate,
            DueDate = cheque.DueDate,
            ChequeAmount = cheque.ChequeAmount,
            ReceiptNo = cheque.ReceiptNo,
            PayeeName = cheque.PayeeName,
            Status = cheque.Status,
            IsVerified = cheque.IsVerified,
            IsOverdue = cheque.IsOverdue,
            Invoices = cheque.Invoices.Select(i => new InvoiceDto
            {
                Id = i.Id,
                InvoiceNo = i.InvoiceNo,
                InvoiceAmount = i.InvoiceAmount
            }).ToList()
        };
    }

    public static ChequeReportDto MapToReportDto(Cheque cheque)
    {
        return new ChequeReportDto
        {
            Id = cheque.Id,
            ChequeId = cheque.ChequeId,
            VendorId = cheque.VendorId,
            VendorName = cheque.Vendor?.VendorName ?? "Unknown",
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
            Status = cheque.Status.ToString(), // ✅ ChequeReportDto uses string, not enum
            IsVerified = cheque.IsVerified,
            IsOverdue = cheque.IsOverdue,
            Invoices = cheque.Invoices.Select(i => new InvoiceDto
            {
                Id = i.Id,
                InvoiceNo = i.InvoiceNo,
                InvoiceAmount = i.InvoiceAmount
            }).ToList()
        };
    }
}
