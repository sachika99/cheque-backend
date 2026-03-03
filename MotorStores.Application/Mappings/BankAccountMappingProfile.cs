using AutoMapper;
using MediatR;
using MotorStores.Application.DTOs;
using MotorStores.Domain.Entities;
using MotorStores.Domain.Enums;

namespace MotorStores.Application.Mappings;
 
public class BankAccountctccctMappingProfile : Profile
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

public static class BankAccountMapper
{
    public static BankAccountDto ToDto(BankAccount account) => new BankAccountDto
    {
        Id = account.Id,
        BankId = account.BankId,
        BankName = account.Bank?.BankName ?? "Unknown",
        BranchName = account.Bank?.BranchName ?? "Unknown",
        AccountNo = account.AccountNo,
        AccountName = account.AccountName,
        AccountType = account.AccountType,
        Balance = account.Balance,
        Status = account.Status.ToString()
    };
}