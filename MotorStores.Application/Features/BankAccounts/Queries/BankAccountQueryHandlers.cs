// Features/BankAccounts/Queries/BankAccountQueryHandlers.cs
using MediatR;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Application.Mappings;

namespace MotorStores.Application.Features.BankAccounts.Queries;

public class GetAllBankAccountsQueryHandler : IRequestHandler<GetAllBankAccountsQuery, IEnumerable<BankAccountDto>>
{
    private readonly IBankAccountRepository _bankAccountRepository;

    public GetAllBankAccountsQueryHandler(IBankAccountRepository bankAccountRepository)
        => _bankAccountRepository = bankAccountRepository;

    public async Task<IEnumerable<BankAccountDto>> Handle(GetAllBankAccountsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var accounts = await _bankAccountRepository.GetAllWithBankAsync(cancellationToken);
            return accounts.Select(BankAccountMapper.ToDto);
        }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

public class GetBankAccountByIdQueryHandler : IRequestHandler<GetBankAccountByIdQuery, BankAccountDto?>
{
    private readonly IBankAccountRepository _bankAccountRepository;

    public GetBankAccountByIdQueryHandler(IBankAccountRepository bankAccountRepository)
        => _bankAccountRepository = bankAccountRepository;

    public async Task<BankAccountDto?> Handle(GetBankAccountByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var account = await _bankAccountRepository.GetByIdWithBankAsync(request.Id, cancellationToken);
            return account == null ? null : BankAccountMapper.ToDto(account);
        }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

public class GetBankAccountsByBankIdQueryHandler : IRequestHandler<GetBankAccountsByBankIdQuery, IEnumerable<BankAccountDto>>
{
    private readonly IBankAccountRepository _bankAccountRepository;

    public GetBankAccountsByBankIdQueryHandler(IBankAccountRepository bankAccountRepository)
        => _bankAccountRepository = bankAccountRepository;

    public async Task<IEnumerable<BankAccountDto>> Handle(GetBankAccountsByBankIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var accounts = await _bankAccountRepository.GetByBankIdAsync(request.BankId, cancellationToken);
            return accounts.Select(BankAccountMapper.ToDto);
        }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}