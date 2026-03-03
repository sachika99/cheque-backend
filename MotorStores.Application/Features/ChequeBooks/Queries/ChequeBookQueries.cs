// Features/ChequeBooks/Queries/ChequeBookQueries.cs
using MediatR;
using MotorStores.Application.DTOs;

namespace MotorStores.Application.Features.ChequeBooks.Queries;

public record GetAllChequeBooksQuery : IRequest<IEnumerable<ChequeBookDto>> { }

public record GetChequeBookByIdQuery : IRequest<ChequeBookDto?>
{
    public int Id { get; init; }
}

public record GetChequeBooksByAccountQuery : IRequest<IEnumerable<ChequeBookDto>>
{
    public int BankAccountId { get; init; }
}