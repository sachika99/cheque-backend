// Features/BankAccounts/Commands/BankAccountCommandHandlers.cs
using MediatR;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Application.Mappings;
using MotorStores.Domain.Entities;
using MotorStores.Domain.Enums;

namespace MotorStores.Application.Features.BankAccounts.Commands;

// ── CREATE ───────────────────────────────────────────────────────────────────
public class CreateBankAccountCommandHandler : IRequestHandler<CreateBankAccountCommand, BankAccountDto>
{
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly IChequeBookRepository _chequeBookRepository;
    private readonly ICurrentUserService _currentUser;

    public CreateBankAccountCommandHandler(
        IBankAccountRepository bankAccountRepository,
        IChequeBookRepository chequeBookRepository,
        ICurrentUserService currentUser)
    {
        _bankAccountRepository = bankAccountRepository;
        _chequeBookRepository = chequeBookRepository;
        _currentUser = currentUser;
    }

    public async Task<BankAccountDto> Handle(CreateBankAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.BankAccount;

            var bankExists = await _bankAccountRepository.BankExistsAsync(dto.BankId, cancellationToken);
            if (!bankExists)
                throw new InvalidOperationException($"Bank with ID {dto.BankId} not found.");

            var account = new BankAccount
            {
                BankId = dto.BankId,
                AccountNo = dto.AccountNo,
                AccountName = dto.AccountName,
                AccountType = dto.AccountType,
                Balance = dto.Balance,
                Status = Enum.Parse<AccountStatus>(dto.Status),
                UserId = _currentUser.UserId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _bankAccountRepository.AddAsync(account, cancellationToken);

            // Auto-create cheque book
            await _chequeBookRepository.AddAsync(new ChequeBook
            {
                UserId = _currentUser.UserId,
                BankAccountId = created.Id,
                ChequeBookNo = "001",
                StartChequeNo = 0,
                EndChequeNo = 0,
                CurrentChequeNo = "0",
                Status = ChequeBookStatus.Active,
                IssuedDate = DateTime.UtcNow
            });

            var withBank = await _bankAccountRepository.GetByIdWithBankAsync(created.Id, cancellationToken);
            return BankAccountMapper.ToDto(withBank!);
        }
        catch (InvalidOperationException) { throw; }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

// ── UPDATE ───────────────────────────────────────────────────────────────────
public class UpdateBankAccountCommandHandler : IRequestHandler<UpdateBankAccountCommand, BankAccountDto>
{
    private readonly IBankAccountRepository _bankAccountRepository;

    public UpdateBankAccountCommandHandler(IBankAccountRepository bankAccountRepository)
        => _bankAccountRepository = bankAccountRepository;

    public async Task<BankAccountDto> Handle(UpdateBankAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.BankAccount;

            var account = await _bankAccountRepository.GetByIdWithBankAsync(dto.Id, cancellationToken)
                ?? throw new InvalidOperationException($"Bank account with ID {dto.Id} not found.");

            if (account.BankId != dto.BankId)
            {
                var bankExists = await _bankAccountRepository.BankExistsAsync(dto.BankId, cancellationToken);
                if (!bankExists)
                    throw new InvalidOperationException($"Bank with ID {dto.BankId} not found.");
            }

            var accountNoExists = await _bankAccountRepository.AccountNoExistsAsync(dto.AccountNo, dto.Id, cancellationToken);
            if (accountNoExists)
                throw new InvalidOperationException($"Another account with number {dto.AccountNo} already exists.");

            account.BankId = dto.BankId;
            account.AccountNo = dto.AccountNo;
            account.AccountName = dto.AccountName;
            account.AccountType = dto.AccountType;
            account.Balance = dto.Balance;
            account.Status = Enum.Parse<AccountStatus>(dto.Status);
            account.UpdatedAt = DateTime.UtcNow;

            var updated = await _bankAccountRepository.UpdateAsync(account, cancellationToken);
            var withBank = await _bankAccountRepository.GetByIdWithBankAsync(updated.Id, cancellationToken);
            return BankAccountMapper.ToDto(withBank!);
        }
        catch (InvalidOperationException) { throw; }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

// ── DELETE ───────────────────────────────────────────────────────────────────
public class DeleteBankAccountCommandHandler : IRequestHandler<DeleteBankAccountCommand, bool>
{
    private readonly IBankAccountRepository _bankAccountRepository;

    public DeleteBankAccountCommandHandler(IBankAccountRepository bankAccountRepository)
        => _bankAccountRepository = bankAccountRepository;

    public async Task<bool> Handle(DeleteBankAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var account = await _bankAccountRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
            if (account == null) return false;

            if (account.Cheques.Any())
                throw new InvalidOperationException("Cannot delete bank account because cheques are linked to this account.");

            await _bankAccountRepository.DeleteAsync(request.Id, cancellationToken);
            return true;
        }
        catch (InvalidOperationException) { throw; }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

// ── ACTIVATE ─────────────────────────────────────────────────────────────────
public class ActivateBankAccountCommandHandler : IRequestHandler<ActivateBankAccountCommand, Unit>
{
    private readonly IBankAccountRepository _bankAccountRepository;

    public ActivateBankAccountCommandHandler(IBankAccountRepository bankAccountRepository)
        => _bankAccountRepository = bankAccountRepository;

    public async Task<Unit> Handle(ActivateBankAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var account = await _bankAccountRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new InvalidOperationException($"Bank account with ID {request.Id} not found.");

            var allAccounts = await _bankAccountRepository.GetAllByBankAsync(cancellationToken);

            foreach (var acc in allAccounts)
            {
                acc.Status = acc.Id == request.Id ? AccountStatus.Active : AccountStatus.Inactive;
                acc.UpdatedAt = DateTime.UtcNow;
                await _bankAccountRepository.UpdateAsync(acc, cancellationToken);
            }

            return Unit.Value;
        }
        catch (InvalidOperationException) { throw; }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}