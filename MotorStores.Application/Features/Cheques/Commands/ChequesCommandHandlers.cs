// Features/Cheques/Commands/ChequeCommandHandlers.cs
using MediatR;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Application.Mappings;
using MotorStores.Domain.Entities;
using MotorStores.Domain.Enums;

namespace MotorStores.Application.Features.Cheques.Commands;

public class CreateChequeCommandHandler : IRequestHandler<CreateChequeCommand, ChequeDto>
{
    private readonly IChequeRepository _chequeRepository;
    private readonly IChequeBookRepository _chequeBookRepository;
    private readonly ICurrentUserService _currentUser;

    public CreateChequeCommandHandler(
        IChequeRepository chequeRepository,
        IChequeBookRepository chequeBookRepository,
        ICurrentUserService currentUser)
    {
        _chequeRepository = chequeRepository;
        _chequeBookRepository = chequeBookRepository;
        _currentUser = currentUser;
    }

    public async Task<ChequeDto> Handle(CreateChequeCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Cheque;

        var cheque = new Cheque
        {
            UserId = _currentUser.UserId,
            ChequeId = dto.ChequeId,
            VendorId = dto.VendorId,
            ChequeBookId = dto.ChequeBookId,
            BankAccountId = dto.BankAccountId,
            InvoiceNo = dto.InvoiceNo,
            InvoiceDate = dto.InvoiceDate,
            InvoiceAmount = dto.InvoiceAmount,
            ReceiptNo = dto.ReceiptNo,
            ChequeNo = dto.ChequeNo,
            ChequeDate = dto.ChequeDate,
            DueDate = dto.DueDate,
            ChequeAmount = dto.ChequeAmount,
            PayeeName = dto.PayeeName,
            Status = dto.Status ?? ChequeStatus.Pending,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var inv in dto.Invoices)
        {
            cheque.Invoices.Add(new Invoice
            {
                UserId = _currentUser.UserId,
                InvoiceNo = inv.InvoiceNo,
                InvoiceAmount = inv.InvoiceAmount
            });
        }

        var created = await _chequeRepository.AddAsync(cheque, cancellationToken);
        await _chequeBookRepository.UpdateCurrentChequeNoAsync(cheque.ChequeBookId, cheque.ChequeNo);

        return ChequeMapper.ToDto(created);
    }
}

// ── UPDATE STATUS ────────────────────────────────────────────────────────────
public class UpdateChequeStatusCommandHandler : IRequestHandler<UpdateChequeStatusCommand, Unit>
{
    private readonly IChequeRepository _chequeRepository;
    private readonly ICurrentUserService _currentUser;

    public UpdateChequeStatusCommandHandler(IChequeRepository chequeRepository, ICurrentUserService currentUser)
    {
        _chequeRepository = chequeRepository;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(UpdateChequeStatusCommand request, CancellationToken cancellationToken)
    {
        var cheque = await _chequeRepository.GetByChequeIdAsync(request.ChequeId, cancellationToken)
            ?? throw new InvalidOperationException($"Cheque with ID {request.ChequeId} not found.");

        if (!Enum.TryParse<ChequeStatus>(request.NewStatus, out var status))
            throw new ArgumentException($"Invalid status: {request.NewStatus}");

        var oldStatus = cheque.Status.ToString();
        cheque.Status = status;
        cheque.UpdatedAt = DateTime.UtcNow;
        cheque.UpdatedBy = _currentUser.UserName;
        if (status == ChequeStatus.Issued) cheque.IssueDate = DateTime.UtcNow;
        if (status == ChequeStatus.Cleared) cheque.ClearedDate = DateTime.UtcNow;

        await _chequeRepository.AddHistoryAsync(new ChequeHistory
        {
            UserId = _currentUser.UserId,
            ChequeId = cheque.Id,
            Action = "Status Changed",
            OldStatus = oldStatus,
            NewStatus = request.NewStatus,
            ChangedBy = request.User,
            Remarks = $"Status changed from {oldStatus} to {request.NewStatus}",
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = _currentUser.UserName
        }, cancellationToken);

        await _chequeRepository.UpdateAsync(cheque, cancellationToken);
        return Unit.Value;
    }
}

// ── UPDATE STATUS BULK ───────────────────────────────────────────────────────
public class UpdateChequeStatusBulkCommandHandler : IRequestHandler<UpdateChequeStatusBulkCommand, Unit>
{
    private readonly IChequeRepository _chequeRepository;
    private readonly ICurrentUserService _currentUser;

    public UpdateChequeStatusBulkCommandHandler(IChequeRepository chequeRepository, ICurrentUserService currentUser)
    {
        _chequeRepository = chequeRepository;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(UpdateChequeStatusBulkCommand request, CancellationToken cancellationToken)
    {
        if (request.ChequeIds == null || request.ChequeIds.Count == 0)
            throw new ArgumentException("ChequeIds cannot be empty.");

        if (!Enum.TryParse<ChequeStatus>(request.NewStatus, out var status))
            throw new ArgumentException($"Invalid status: {request.NewStatus}");

        var cheques = await _chequeRepository.GetByIdsAsync(request.ChequeIds, cancellationToken);
        var chequeList = cheques.ToList();

        if (chequeList.Count != request.ChequeIds.Count)
            throw new InvalidOperationException("One or more cheques not found.");

        foreach (var cheque in chequeList)
        {
            var oldStatus = cheque.Status.ToString();
            if (oldStatus == request.NewStatus) continue;

            cheque.Status = status;
            cheque.UpdatedAt = DateTime.UtcNow;
            if (status == ChequeStatus.Issued) cheque.IssueDate = DateTime.UtcNow;
            if (status == ChequeStatus.Cleared) cheque.ClearedDate = DateTime.UtcNow;

            await _chequeRepository.AddHistoryAsync(new ChequeHistory
            {
                UserId = _currentUser.UserId,
                ChequeId = cheque.Id,
                Action = "Status Changed",
                OldStatus = oldStatus,
                NewStatus = request.NewStatus,
                ChangedBy = request.User,
                Remarks = $"Status changed from {oldStatus} to {request.NewStatus}",
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);

            await _chequeRepository.UpdateAsync(cheque, cancellationToken);
        }

        return Unit.Value;
    }
}

// ── UPDATE CHEQUE ────────────────────────────────────────────────────────────
public class UpdateChequeCommandHandler : IRequestHandler<UpdateChequeCommand, Unit>
{
    private readonly IChequeRepository _chequeRepository;
    private readonly ICurrentUserService _currentUser;

    public UpdateChequeCommandHandler(IChequeRepository chequeRepository, ICurrentUserService currentUser)
    {
        _chequeRepository = chequeRepository;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(UpdateChequeCommand request, CancellationToken cancellationToken)
    {
        var cheque = await _chequeRepository.GetByChequeIdWithDetailsAsync(request.ChequeId, cancellationToken)
            ?? throw new InvalidOperationException($"Cheque with ID {request.ChequeId} not found.");

        var r = request.Request;
        cheque.ChequeBookId = r.ChequeBookId;
        cheque.BankAccountId = r.BankAccountId;
        cheque.InvoiceDate = r.InvoiceDate;
        cheque.InvoiceAmount = r.InvoiceAmount;
        cheque.ChequeNo = r.ChequeNo;
        cheque.ChequeDate = r.ChequeDate;
        cheque.DueDate = r.DueDate;
        cheque.ChequeAmount = r.ChequeAmount;
        cheque.PayeeName = r.PayeeName;
        cheque.UpdatedAt = DateTime.UtcNow;

        var incomingIds = r.Invoices.Where(i => i.Id > 0).Select(i => i.Id).ToList();
        var toDelete = cheque.Invoices.Where(i => !incomingIds.Contains(i.Id)).ToList();
        await _chequeRepository.RemoveInvoicesAsync(toDelete);

        foreach (var inv in r.Invoices)
        {
            var existing = cheque.Invoices.FirstOrDefault(i =>
                (inv.Id > 0 && i.Id == inv.Id) ||
                (!string.IsNullOrWhiteSpace(inv.InvoiceNo) && i.InvoiceNo == inv.InvoiceNo));

            if (existing != null)
            {
                existing.InvoiceNo = inv.InvoiceNo;
                existing.InvoiceAmount = inv.InvoiceAmount;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                await _chequeRepository.AddInvoiceAsync(new Invoice
                {
                    UserId = _currentUser.UserId,
                    ChequeId = cheque.Id,
                    InvoiceNo = inv.InvoiceNo,
                    InvoiceAmount = inv.InvoiceAmount,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await _chequeRepository.UpdateAsync(cheque, cancellationToken);
        return Unit.Value;
    }
}

// ── MARK VERIFIED ────────────────────────────────────────────────────────────
public class MarkChequeAsVerifiedCommandHandler : IRequestHandler<MarkChequeAsVerifiedCommand, Unit>
{
    private readonly IChequeRepository _chequeRepository;
    private readonly ICurrentUserService _currentUser;

    public MarkChequeAsVerifiedCommandHandler(IChequeRepository chequeRepository, ICurrentUserService currentUser)
    {
        _chequeRepository = chequeRepository;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(MarkChequeAsVerifiedCommand request, CancellationToken cancellationToken)
    {
        var cheque = await _chequeRepository.GetByChequeIdAsync(request.ChequeId, cancellationToken)
            ?? throw new InvalidOperationException($"Cheque with ID {request.ChequeId} not found.");

        cheque.IsVerified = true;
        cheque.UpdatedAt = DateTime.UtcNow;

        await _chequeRepository.AddHistoryAsync(new ChequeHistory
        {
            UserId = _currentUser.UserId,
            ChequeId = cheque.Id,
            Action = "Verified",
            OldStatus = cheque.Status.ToString(),
            NewStatus = cheque.Status.ToString(),
            ChangedBy = "System",
            Remarks = "Cheque marked as verified after reconciliation",
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _chequeRepository.UpdateAsync(cheque, cancellationToken);
        return Unit.Value;
    }
}

// ── DELETE ───────────────────────────────────────────────────────────────────
public class DeleteChequeCommandHandler : IRequestHandler<DeleteChequeCommand, Unit>
{
    private readonly IChequeRepository _chequeRepository;

    public DeleteChequeCommandHandler(IChequeRepository chequeRepository)
        => _chequeRepository = chequeRepository;

    public async Task<Unit> Handle(DeleteChequeCommand request, CancellationToken cancellationToken)
    {
        var cheque = await _chequeRepository.GetByIdWithDetailsAsync(request.ChequeId, cancellationToken)
            ?? throw new InvalidOperationException($"Cheque with ID {request.ChequeId} not found.");

        if (cheque.Invoices.Any())
            await _chequeRepository.RemoveInvoicesAsync(cheque.Invoices);

        await _chequeRepository.DeleteHistoriesByChequeAsync(cheque.Id, cancellationToken);
        await _chequeRepository.DeleteAsync(cheque.Id, cancellationToken);

        return Unit.Value;
    }
}