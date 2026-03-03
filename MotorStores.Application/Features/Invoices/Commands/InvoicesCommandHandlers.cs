// Features/Invoices/Commands/InvoiceCommandHandlers.cs
using MediatR;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Application.Mappings;
using MotorStores.Domain.Entities;

namespace MotorStores.Application.Features.Invoices.Commands;

// ── CREATE ───────────────────────────────────────────────────────────────────
public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, InvoiceDto>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICurrentUserService _currentUser;

    public CreateInvoiceCommandHandler(IInvoiceRepository invoiceRepository,
        ICurrentUserService currentUser)
    { _invoiceRepository = invoiceRepository;
        _currentUser = currentUser;
    }
        

    public async Task<InvoiceDto> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var invoice = new Invoice
            {
                InvoiceNo = request.Invoice.InvoiceNo,
                InvoiceAmount = request.Invoice.InvoiceAmount,
                UserId = _currentUser.UserId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _invoiceRepository.AddAsync(invoice, cancellationToken);
            return InvoiceMapper.ToDto(created);
        }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

// ── UPDATE ───────────────────────────────────────────────────────────────────
public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, InvoiceDto>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public UpdateInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
        => _invoiceRepository = invoiceRepository;

    public async Task<InvoiceDto> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.Invoice.Id, cancellationToken)
                ?? throw new InvalidOperationException($"Invoice with ID {request.Invoice.Id} not found.");

            invoice.InvoiceNo = request.Invoice.InvoiceNo;
            invoice.InvoiceAmount = request.Invoice.InvoiceAmount;
            invoice.UpdatedAt = DateTime.UtcNow;

            var updated = await _invoiceRepository.UpdateAsync(invoice, cancellationToken);
            return InvoiceMapper.ToDto(updated);
        }
        catch (InvalidOperationException) { throw; }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

// ── DELETE ───────────────────────────────────────────────────────────────────
public class DeleteInvoiceCommandHandler : IRequestHandler<DeleteInvoiceCommand, bool>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public DeleteInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
        => _invoiceRepository = invoiceRepository;

    public async Task<bool> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.Id, cancellationToken);
            if (invoice == null) return false;

            await _invoiceRepository.DeleteAsync(request.Id, cancellationToken);
            return true;
        }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}