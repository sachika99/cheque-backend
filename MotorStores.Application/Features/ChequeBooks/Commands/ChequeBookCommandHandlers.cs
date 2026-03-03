// Features/ChequeBooks/Commands/ChequeBookCommandHandlers.cs
using MediatR;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Application.Mappings;
using MotorStores.Domain.Entities;
using MotorStores.Domain.Enums;

namespace MotorStores.Application.Features.ChequeBooks.Commands;

// ── CREATE ───────────────────────────────────────────────────────────────────
public class CreateChequeBookCommandHandler : IRequestHandler<CreateChequeBookCommand, ChequeBookDto>
{
    private readonly IChequeBookRepository _chequeBookRepository;
    private readonly ICurrentUserService _currentUser;

    public CreateChequeBookCommandHandler(IChequeBookRepository chequeBookRepository,
        ICurrentUserService currentUser)

    { _chequeBookRepository = chequeBookRepository;
        _currentUser = currentUser; }

    public async Task<ChequeBookDto> Handle(CreateChequeBookCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.ChequeBook;

            var accountExists = await _chequeBookRepository.BankAccountExistsAsync(dto.BankAccountId, cancellationToken);
            if (!accountExists)
                throw new InvalidOperationException($"Bank account with ID {dto.BankAccountId} not found.");

            var chequeBook = new ChequeBook
            {
                BankAccountId = dto.BankAccountId,
                ChequeBookNo = dto.ChequeBookNo,
                StartChequeNo = dto.StartChequeNo,
                EndChequeNo = dto.EndChequeNo,
                CurrentChequeNo = dto.CurrentChequeNo,
                Status = ChequeBookStatus.Active,
                IssuedDate = dto.IssuedDate,
                UserId = _currentUser.UserId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _chequeBookRepository.AddAsync(chequeBook, cancellationToken);
            var withAccount = await _chequeBookRepository.GetByIdWithAccountAsync(created.Id, cancellationToken);
            return ChequeBookMapper.ToDto(withAccount!);
        }
        catch (InvalidOperationException) { throw; }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

// ── UPDATE ───────────────────────────────────────────────────────────────────
public class UpdateChequeBookCommandHandler : IRequestHandler<UpdateChequeBookCommand, ChequeBookDto>
{
    private readonly IChequeBookRepository _chequeBookRepository;

    public UpdateChequeBookCommandHandler(IChequeBookRepository chequeBookRepository)
        => _chequeBookRepository = chequeBookRepository;

    public async Task<ChequeBookDto> Handle(UpdateChequeBookCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.ChequeBook;

            var chequeBook = await _chequeBookRepository.GetByIdWithAccountAsync(dto.Id, cancellationToken)
                ?? throw new InvalidOperationException($"Cheque book with ID {dto.Id} not found.");

            if (chequeBook.BankAccountId != dto.BankAccountId)
            {
                var accountExists = await _chequeBookRepository.BankAccountExistsAsync(dto.BankAccountId, cancellationToken);
                if (!accountExists)
                    throw new InvalidOperationException($"Bank account with ID {dto.BankAccountId} not found.");
            }

            var duplicateExists = await _chequeBookRepository.ChequeBookNoExistsAsync(dto.ChequeBookNo, dto.Id, cancellationToken);
            if (duplicateExists)
                throw new InvalidOperationException($"Another cheque book with number {dto.ChequeBookNo} already exists.");

            if (dto.StartChequeNo >= dto.EndChequeNo)
                throw new ArgumentException("Start cheque number must be less than end cheque number.");

            //if (dto.CurrentChequeNo < dto.StartChequeNo || dto.CurrentChequeNo > dto.EndChequeNo)
            //    throw new ArgumentException("Current cheque number must be within the cheque book range.");

            chequeBook.BankAccountId = dto.BankAccountId;
            chequeBook.ChequeBookNo = dto.ChequeBookNo;
            chequeBook.StartChequeNo = dto.StartChequeNo;
            chequeBook.EndChequeNo = dto.EndChequeNo;
            chequeBook.CurrentChequeNo = dto.CurrentChequeNo;
            chequeBook.Status = Enum.Parse<ChequeBookStatus>(dto.Status);
            chequeBook.IssuedDate = dto.IssuedDate;
            chequeBook.UpdatedAt = DateTime.UtcNow;

            //if (chequeBook.CurrentChequeNo >= chequeBook.EndChequeNo)
            //    chequeBook.Status = ChequeBookStatus.Completed;

            var updated = await _chequeBookRepository.UpdateAsync(chequeBook, cancellationToken);
            var withAccount = await _chequeBookRepository.GetByIdWithAccountAsync(updated.Id, cancellationToken);
            return ChequeBookMapper.ToDto(withAccount!);
        }
        catch (InvalidOperationException) { throw; }
        catch (ArgumentException) { throw; }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

// ── DELETE ───────────────────────────────────────────────────────────────────
public class DeleteChequeBookCommandHandler : IRequestHandler<DeleteChequeBookCommand, bool>
{
    private readonly IChequeBookRepository _chequeBookRepository;

    public DeleteChequeBookCommandHandler(IChequeBookRepository chequeBookRepository)
        => _chequeBookRepository = chequeBookRepository;

    public async Task<bool> Handle(DeleteChequeBookCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var chequeBook = await _chequeBookRepository.GetByIdWithChequesAsync(request.Id, cancellationToken);
            if (chequeBook == null) return false;

            if (chequeBook.Cheques.Any())
                throw new InvalidOperationException("Cannot delete cheque book with associated cheques.");

            await _chequeBookRepository.DeleteAsync(request.Id, cancellationToken);
            return true;
        }
        catch (InvalidOperationException) { throw; }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

// ── GET NEXT CHEQUE NUMBER ────────────────────────────────────────────────────
//public class GetNextChequeNumberCommandHandler : IRequestHandler<GetNextChequeNumberCommand, string>
//{
//    private readonly IChequeBookRepository _chequeBookRepository;

//    public GetNextChequeNumberCommandHandler(IChequeBookRepository chequeBookRepository)
//        => _chequeBookRepository = chequeBookRepository;

//    public async Task<string> Handle(GetNextChequeNumberCommand request, CancellationToken cancellationToken)
//    {
//        try
//        {
//            var chequeBook = await _chequeBookRepository.GetByIdAsync(request.ChequeBookId, cancellationToken)
//                ?? throw new InvalidOperationException($"Cheque book with ID {request.ChequeBookId} not found.");

//            if (chequeBook.Status != ChequeBookStatus.Active)
//                throw new InvalidOperationException("Cheque book is not active.");

//            //if (chequeBook.CurrentChequeNo >= chequeBook.EndChequeNo)
//            //    throw new InvalidOperationException("No more cheques available in this cheque book.");

//            //var nextChequeNo = chequeBook.CurrentChequeNo.ToString().PadLeft(6, '0');
//            //chequeBook.CurrentChequeNo++;

//            //if (chequeBook.CurrentChequeNo >= chequeBook.EndChequeNo)
//            //    chequeBook.Status = ChequeBookStatus.Completed;

//            chequeBook.UpdatedAt = DateTime.UtcNow;
//            await _chequeBookRepository.UpdateAsync(chequeBook, cancellationToken);

//            return nextChequeNo;
//        }
//        catch (InvalidOperationException) { throw; }
//        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
//    }
//}

// ── UPDATE CURRENT CHEQUE NO ─────────────────────────────────────────────────
public class UpdateCurrentChequeNoCommandHandler : IRequestHandler<UpdateCurrentChequeNoCommand, ChequeBookDto>
{
    private readonly IChequeBookRepository _chequeBookRepository;

    public UpdateCurrentChequeNoCommandHandler(IChequeBookRepository chequeBookRepository)
        => _chequeBookRepository = chequeBookRepository;

    public async Task<ChequeBookDto> Handle(UpdateCurrentChequeNoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var chequeBook = await _chequeBookRepository.GetByIdWithAccountAsync(request.ChequeBookId, cancellationToken)
                ?? throw new InvalidOperationException($"Cheque book with ID {request.ChequeBookId} not found.");

            chequeBook.CurrentChequeNo = request.CurrentChequeNo;
            chequeBook.UpdatedAt = DateTime.UtcNow;

            var updated = await _chequeBookRepository.UpdateAsync(chequeBook, cancellationToken);
            return ChequeBookMapper.ToDto(updated);
        }
        catch (InvalidOperationException) { throw; }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}