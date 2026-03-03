// Features/Banks/Queries/BankQueries.cs
using MediatR;
using MotorStores.Application.DTOs;

namespace MotorStores.Application.Features.Banks.Queries;

public record GetAllBanksQuery : IRequest<IEnumerable<BankDto>> { }

public record GetBankByIdQuery : IRequest<BankDto?>
{
    public int Id { get; init; }
}