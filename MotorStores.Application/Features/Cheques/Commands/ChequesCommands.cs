
using MediatR;
using MotorStores.Application.DTOs;

namespace MotorStores.Application.Features.Cheques.Commands;

public record CreateChequeCommand : IRequest<ChequeDto>
{
    public ChequeDto Cheque { get; init; } = null!;
}

public record UpdateChequeStatusCommand : IRequest<Unit>
{
    public string ChequeId { get; init; } = null!;
    public string NewStatus { get; init; } = null!;
    public string User { get; init; } = "System";
}

public record UpdateChequeStatusBulkCommand : IRequest<Unit>
{
    public List<string> ChequeIds { get; init; } = new List<string>();
    public string NewStatus { get; init; } = null!;
    public string User { get; init; } = "System";
}

public record UpdateChequeCommand : IRequest<Unit>
{
    public string ChequeId { get; init; } = null!;
    public UpdateChequeRequest Request { get; init; } = null!;
}

public record MarkChequeAsVerifiedCommand : IRequest<Unit>
{
    public string ChequeId { get; init; } = null!;
}

public record DeleteChequeCommand : IRequest<Unit>
{
    public int ChequeId { get; init; }
}