// Features/Banks/Commands/BankCommands.cs
using MediatR;
using MotorStores.Application.DTOs;

namespace MotorStores.Application.Features.Banks.Commands;

public record CreateBankCommand : IRequest<BankDto>
{
    public BankDto Bank { get; init; } = null!;
}

public record CreateBankWithAccountsCommand : IRequest<BankDto>
{
    public BankWithAccountsDto BankWithAccounts { get; init; } = null!;
}

public record UpdateBankCommand : IRequest<BankDto>
{
    public BankDto Bank { get; init; } = null!;
}

public record DeleteBankCommand : IRequest<bool>
{
    public int Id { get; init; }
}