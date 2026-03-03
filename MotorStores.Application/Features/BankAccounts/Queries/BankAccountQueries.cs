// Features/BankAccounts/Queries/BankAccountQueries.cs
using MediatR;
using MotorStores.Application.DTOs;

namespace MotorStores.Application.Features.BankAccounts.Queries;

public record GetAllBankAccountsQuery : IRequest<IEnumerable<BankAccountDto>> { }

public record GetBankAccountByIdQuery : IRequest<BankAccountDto?>
{
    public int Id { get; init; }
}

public record GetBankAccountsByBankIdQuery : IRequest<IEnumerable<BankAccountDto>>
{
    public int BankId { get; init; }
}