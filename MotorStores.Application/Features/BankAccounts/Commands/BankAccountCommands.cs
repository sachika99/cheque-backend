// Features/BankAccounts/Commands/BankAccountCommands.cs
using MediatR;
using MotorStores.Application.DTOs;

namespace MotorStores.Application.Features.BankAccounts.Commands;

public record CreateBankAccountCommand : IRequest<BankAccountDto>
{
    public BankAccountDto BankAccount { get; init; } = null!;
}

public record UpdateBankAccountCommand : IRequest<BankAccountDto>
{
    public BankAccountDto BankAccount { get; init; } = null!;
}

public record DeleteBankAccountCommand : IRequest<bool>
{
    public int Id { get; init; }
}

public record ActivateBankAccountCommand : IRequest<Unit>
{
    public int Id { get; init; }
}