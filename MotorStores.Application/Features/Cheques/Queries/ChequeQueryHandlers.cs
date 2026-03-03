// Features/Cheques/Queries/ChequeQueryHandlers.cs
using MediatR;
using MotorStores.Application.DTOs;
using MotorStores.Application.Features.UserIds.Queries;
using MotorStores.Application.Interfaces;
using MotorStores.Application.Mappings;

namespace MotorStores.Application.Features.Cheques.Queries;

public class GetAllChequesQueryHandler : IRequestHandler<GetAllChequesQuery, IEnumerable<ChequeDto>>
{
    private readonly IChequeRepository _chequeRepository;
    public GetAllChequesQueryHandler(IChequeRepository chequeRepository) => _chequeRepository = chequeRepository;

    public async Task<IEnumerable<ChequeDto>> Handle(GetAllChequesQuery request, CancellationToken cancellationToken)
        => (await _chequeRepository.GetAllAsync(cancellationToken)).Select(ChequeMapper.ToDto);
}

public class GetChequeByIdQueryHandler : IRequestHandler<GetChequeByIdQuery, ChequeReportDto?>
{
    private readonly IChequeRepository _chequeRepository;
    public GetChequeByIdQueryHandler(IChequeRepository chequeRepository) => _chequeRepository = chequeRepository;

    public async Task<ChequeReportDto?> Handle(GetChequeByIdQuery request, CancellationToken cancellationToken)
    {
        var cheque = await _chequeRepository.GetByChequeIdWithDetailsAsync(request.Id, cancellationToken);
        return cheque == null ? null : ChequeMapper.MapToReportDto(cheque);
    }
}

public class GetDueThisMonthQueryHandler : IRequestHandler<GetDueThisMonthQuery, IEnumerable<ChequeReportDto>>
{
    private readonly IChequeRepository _chequeRepository;
    public GetDueThisMonthQueryHandler(IChequeRepository chequeRepository) => _chequeRepository = chequeRepository;

    public async Task<IEnumerable<ChequeReportDto>> Handle(GetDueThisMonthQuery request, CancellationToken cancellationToken)
        => (await _chequeRepository.GetDueThisMonthAsync(cancellationToken)).Select(ChequeMapper.MapToReportDto);
}

public class GetOverdueChequesQueryHandler : IRequestHandler<GetOverdueChequesQuery, IEnumerable<ChequeReportDto>>
{
    private readonly IChequeRepository _chequeRepository;
    public GetOverdueChequesQueryHandler(IChequeRepository chequeRepository) => _chequeRepository = chequeRepository;

    public async Task<IEnumerable<ChequeReportDto>> Handle(GetOverdueChequesQuery request, CancellationToken cancellationToken)
        => (await _chequeRepository.GetOverdueAsync(cancellationToken)).Select(ChequeMapper.MapToReportDto);
}

public class GetClearedChequesQueryHandler : IRequestHandler<GetClearedChequesQuery, IEnumerable<ChequeReportDto>>
{
    private readonly IChequeRepository _chequeRepository;
    public GetClearedChequesQueryHandler(IChequeRepository chequeRepository) => _chequeRepository = chequeRepository;

    public async Task<IEnumerable<ChequeReportDto>> Handle(GetClearedChequesQuery request, CancellationToken cancellationToken)
        => (await _chequeRepository.GetClearedAsync(cancellationToken)).Select(ChequeMapper.MapToReportDto);
}

public class GetStatusSummaryByBankAccountQueryHandler
    : IRequestHandler<GetStatusSummaryByBankAccountQuery, IEnumerable<ChequeStatusSummaryDto>>
{
    private readonly IChequeRepository _chequeRepository;
    public GetStatusSummaryByBankAccountQueryHandler(IChequeRepository chequeRepository) => _chequeRepository = chequeRepository;

    public async Task<IEnumerable<ChequeStatusSummaryDto>> Handle(GetStatusSummaryByBankAccountQuery request, CancellationToken cancellationToken)
        => await _chequeRepository.GetStatusSummaryByBankAccountAsync(request.BankAccountId, cancellationToken);
}

public class GetStatusSummaryByBankAccountTimeQueryHandler
    : IRequestHandler<GetStatusSummaryByBankAccountTimeQuery, IEnumerable<ChequeStatusSummaryDto>>
{
    private readonly IChequeRepository _chequeRepository;
    public GetStatusSummaryByBankAccountTimeQueryHandler(IChequeRepository chequeRepository) => _chequeRepository = chequeRepository;

    public async Task<IEnumerable<ChequeStatusSummaryDto>> Handle(GetStatusSummaryByBankAccountTimeQuery request, CancellationToken cancellationToken)
        => await _chequeRepository.GetStatusSummaryByBankAccountTimeAsync(request.BankAccountId, request.StartDate, request.EndDate, cancellationToken);
}
public class GetCurrentMonthTotalByAccountQueryHandler : IRequestHandler<GetCurrentMonthTotalByAccountQuery, decimal>
{
    private readonly IChequeRepository _chequeRepository;
    public GetCurrentMonthTotalByAccountQueryHandler(IChequeRepository chequeRepository) => _chequeRepository = chequeRepository;

    public async Task<decimal> Handle(GetCurrentMonthTotalByAccountQuery request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"BankAccountId: {request.BankAccountId}");
        Console.WriteLine($"StartDate: {request.StartDate}");
        Console.WriteLine($"EndDate: {request.EndDate}");

        var result = await _chequeRepository.GetCurrentMonthTotalByAccountAsync(
            request.BankAccountId, request.StartDate, request.EndDate, cancellationToken);

        Console.WriteLine($"Result: {result}");

        return result;
    }
}