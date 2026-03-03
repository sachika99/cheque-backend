// Features/Invoices/Commands/InvoiceCommands.cs
using MediatR;
using MotorStores.Application.DTOs;

namespace MotorStores.Application.Features.Invoices.Commands;

public record CreateInvoiceCommand : IRequest<InvoiceDto>
{
    public InvoiceDto Invoice { get; init; } = null!;
}

public record UpdateInvoiceCommand : IRequest<InvoiceDto>
{
    public InvoiceDto Invoice { get; init; } = null!;
}

public record DeleteInvoiceCommand : IRequest<bool>
{
    public int Id { get; init; }
}