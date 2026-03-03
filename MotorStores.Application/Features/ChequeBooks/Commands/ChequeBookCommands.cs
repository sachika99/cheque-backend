// Features/ChequeBooks/Commands/ChequeBookCommands.cs
using MediatR;
using MotorStores.Application.DTOs;

namespace MotorStores.Application.Features.ChequeBooks.Commands;

public record CreateChequeBookCommand : IRequest<ChequeBookDto>
{
    public ChequeBookDto ChequeBook { get; init; } = null!;
}

public record UpdateChequeBookCommand : IRequest<ChequeBookDto>
{
    public ChequeBookDto ChequeBook { get; init; } = null!;
}

public record DeleteChequeBookCommand : IRequest<bool>
{
    public int Id { get; init; }
}

public record GetNextChequeNumberCommand : IRequest<string>
{
    public int ChequeBookId { get; init; }
}

public record UpdateCurrentChequeNoCommand : IRequest<ChequeBookDto>
{
    public int ChequeBookId { get; init; }
    public string CurrentChequeNo { get; init; }
}