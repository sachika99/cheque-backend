// Features/ChequeBooks/Queries/ChequeBookQueryHandlers.cs
using MediatR;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Application.Mappings;

namespace MotorStores.Application.Features.ChequeBooks.Queries;

public class GetAllChequeBooksQueryHandler : IRequestHandler<GetAllChequeBooksQuery, IEnumerable<ChequeBookDto>>
{
    private readonly IChequeBookRepository _chequeBookRepository;

    public GetAllChequeBooksQueryHandler(IChequeBookRepository chequeBookRepository)
        => _chequeBookRepository = chequeBookRepository;

    public async Task<IEnumerable<ChequeBookDto>> Handle(GetAllChequeBooksQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var chequeBooks = await _chequeBookRepository.GetAllWithAccountAsync(cancellationToken);
            return chequeBooks.Select(ChequeBookMapper.ToDto);
        }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

public class GetChequeBookByIdQueryHandler : IRequestHandler<GetChequeBookByIdQuery, ChequeBookDto?>
{
    private readonly IChequeBookRepository _chequeBookRepository;

    public GetChequeBookByIdQueryHandler(IChequeBookRepository chequeBookRepository)
        => _chequeBookRepository = chequeBookRepository;

    public async Task<ChequeBookDto?> Handle(GetChequeBookByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var chequeBook = await _chequeBookRepository.GetByIdWithAccountAsync(request.Id, cancellationToken);
            return chequeBook == null ? null : ChequeBookMapper.ToDto(chequeBook);
        }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

public class GetChequeBooksByAccountQueryHandler : IRequestHandler<GetChequeBooksByAccountQuery, IEnumerable<ChequeBookDto>>
{
    private readonly IChequeBookRepository _chequeBookRepository;

    public GetChequeBooksByAccountQueryHandler(IChequeBookRepository chequeBookRepository)
        => _chequeBookRepository = chequeBookRepository;

    public async Task<IEnumerable<ChequeBookDto>> Handle(GetChequeBooksByAccountQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var chequeBooks = await _chequeBookRepository.GetByBankAccountIdAsync(request.BankAccountId, cancellationToken);
            return chequeBooks.Select(ChequeBookMapper.ToDto);
        }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}