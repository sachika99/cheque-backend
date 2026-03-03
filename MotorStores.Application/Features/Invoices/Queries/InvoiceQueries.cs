// Features/Invoices/Queries/InvoiceQueries.cs
using MediatR;
using MotorStores.Application.DTOs;

namespace MotorStores.Application.Features.Invoices.Queries;

public record GetAllInvoicesQuery : IRequest<IEnumerable<InvoiceDto>> { }

public record GetInvoiceByIdQuery : IRequest<InvoiceDto?>
{
    public int Id { get; init; }
}