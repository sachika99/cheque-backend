// Features/Invoices/Queries/InvoiceQueryHandlers.cs
using MediatR;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Application.Mappings;

namespace MotorStores.Application.Features.Invoices.Queries;

// ── GET ALL ──────────────────────────────────────────────────────────────────
public class GetAllInvoicesQueryHandler : IRequestHandler<GetAllInvoicesQuery, IEnumerable<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetAllInvoicesQueryHandler(IInvoiceRepository invoiceRepository)
        => _invoiceRepository = invoiceRepository;

    public async Task<IEnumerable<InvoiceDto>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var invoices = await _invoiceRepository.GetAllOrderedAsync(cancellationToken);
            return invoices.Select(InvoiceMapper.ToDto);
        }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

// ── GET BY ID ────────────────────────────────────────────────────────────────
public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto?>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoiceByIdQueryHandler(IInvoiceRepository invoiceRepository)
        => _invoiceRepository = invoiceRepository;

    public async Task<InvoiceDto?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.Id, cancellationToken);
            return invoice == null ? null : InvoiceMapper.ToDto(invoice);
        }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}