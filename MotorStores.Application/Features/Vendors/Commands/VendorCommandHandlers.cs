using MediatR;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Application.Mappings;
using MotorStores.Domain.Entities;

namespace MotorStores.Application.Features.Vendors.Commands;

public class CreateVendorCommandHandler : IRequestHandler<CreateVendorCommand, VendorDto>
{
    private readonly IVendorRepository _vendorRepository;

    public CreateVendorCommandHandler(IVendorRepository vendorRepository)
    {
        _vendorRepository = vendorRepository;
    }

    public async Task<VendorDto> Handle(CreateVendorCommand request, CancellationToken cancellationToken)
    {
        
        var vendorCode = request.Vendor.VendorCode;
        if (string.IsNullOrWhiteSpace(vendorCode))
        {
             
            vendorCode = await GenerateVendorCodeAsync(cancellationToken);
        }

        var existingVendor = await _vendorRepository.GetByVendorCodeAsync(vendorCode, cancellationToken);
        if (existingVendor != null)
        {
            throw new InvalidOperationException($"Vendor with code '{vendorCode}' already exists.");
        }

        var vendor = new Vendor
        {
            VendorCode = vendorCode,
            VendorName = request.Vendor.VendorName,
            VendorAddress = request.Vendor.VendorAddress,
            VendorPhoneNo = request.Vendor.VendorPhoneNo,
            VendorEmail = request.Vendor.VendorEmail,
            BankName = request.Vendor.BankName,
            AccountNumber = request.Vendor.AccountNumber,
            CrediPeriodDays = request.Vendor.CrediPeriodDays,
            Notes = request.Vendor.Notes,
            ContactPerson = request.Vendor.ContactPerson,
            Status = Domain.Enums.VendorStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var createdVendor = await _vendorRepository.AddAsync(vendor, cancellationToken);
        return VendorMapper.ToDto(createdVendor);
    }

    private async Task<string> GenerateVendorCodeAsync(CancellationToken cancellationToken)
    { 
        var lastVendor = await _vendorRepository.GetLastVendorAsync(cancellationToken);
        
        if (lastVendor == null || string.IsNullOrEmpty(lastVendor.VendorCode))
        {
            return "VENDOR001";
        }
  
        var lastCode = lastVendor.VendorCode;
        if (lastCode.StartsWith("VENDOR", StringComparison.OrdinalIgnoreCase))
        {
            var numberPart = lastCode.Substring(6);  
            if (int.TryParse(numberPart, out int lastNumber))
            {
                var nextNumber = lastNumber + 1;
                return $"VENDOR{nextNumber:D3}";  
            }
        }
 
        var allVendors = await _vendorRepository.GetAllAsync(cancellationToken);
        var sequentialNextNumber = allVendors.Count() + 1;
        return $"VENDOR{sequentialNextNumber:D3}";
    }
}

public class UpdateVendorCommandHandler : IRequestHandler<UpdateVendorCommand, VendorDto>
{
    private readonly IVendorRepository _vendorRepository;

    public UpdateVendorCommandHandler(IVendorRepository vendorRepository)
    {
        _vendorRepository = vendorRepository;
    }

    public async Task<VendorDto> Handle(UpdateVendorCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _vendorRepository.GetByIdAsync(request.Vendor.Id, cancellationToken);
        if (vendor == null)
        {
            throw new InvalidOperationException($"Vendor with ID {request.Vendor.Id} not found.");
        }
 
        var codeExists = await _vendorRepository.VendorCodeExistsAsync(request.Vendor.VendorCode, request.Vendor.Id, cancellationToken);
        if (codeExists)
        {
            throw new InvalidOperationException($"Vendor with code '{request.Vendor.VendorCode}' already exists.");
        }

        vendor.VendorCode = request.Vendor.VendorCode;
        vendor.VendorName = request.Vendor.VendorName;
        vendor.VendorAddress = request.Vendor.VendorAddress;
        vendor.VendorPhoneNo = request.Vendor.VendorPhoneNo;
        vendor.VendorEmail = request.Vendor.VendorEmail;
        vendor.BankName = request.Vendor.BankName;
        vendor.AccountNumber = request.Vendor.AccountNumber;
        vendor.CrediPeriodDays = request.Vendor.CrediPeriodDays;
        vendor.Status = request.Vendor.Status;
        vendor.Notes = request.Vendor.Notes;
        vendor.ContactPerson = request.Vendor.ContactPerson;
        vendor.UpdatedAt = DateTime.UtcNow;

        var updatedVendor = await _vendorRepository.UpdateAsync(vendor, cancellationToken);
        return VendorMapper.ToDto(updatedVendor);
    }
}

public class DeleteVendorCommandHandler : IRequestHandler<DeleteVendorCommand, bool>
{
    private readonly IVendorRepository _vendorRepository;

    public DeleteVendorCommandHandler(IVendorRepository vendorRepository)
    {
        _vendorRepository = vendorRepository;
    }

    public async Task<bool> Handle(DeleteVendorCommand request, CancellationToken cancellationToken)
    {
        return await _vendorRepository.DeleteAsync(request.Id, cancellationToken);
    }
}

public class ActivateVendorCommandHandler : IRequestHandler<ActivateVendorCommand, VendorDto>
{
    private readonly IVendorRepository _vendorRepository;

    public ActivateVendorCommandHandler(IVendorRepository vendorRepository)
    {
        _vendorRepository = vendorRepository;
    }

    public async Task<VendorDto> Handle(ActivateVendorCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _vendorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (vendor == null)
        {
            throw new InvalidOperationException($"Vendor with ID {request.Id} not found.");
        }

        vendor.Activate();
        var updatedVendor = await _vendorRepository.UpdateAsync(vendor, cancellationToken);
        return VendorMapper.ToDto(updatedVendor);
    }
}

public class DeactivateVendorCommandHandler : IRequestHandler<DeactivateVendorCommand, VendorDto>
{
    private readonly IVendorRepository _vendorRepository;

    public DeactivateVendorCommandHandler(IVendorRepository vendorRepository)
    {
        _vendorRepository = vendorRepository;
    }

    public async Task<VendorDto> Handle(DeactivateVendorCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _vendorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (vendor == null)
        {
            throw new InvalidOperationException($"Vendor with ID {request.Id} not found.");
        }

        vendor.Deactivate();
        var updatedVendor = await _vendorRepository.UpdateAsync(vendor, cancellationToken);
        return VendorMapper.ToDto(updatedVendor);
    }
}
