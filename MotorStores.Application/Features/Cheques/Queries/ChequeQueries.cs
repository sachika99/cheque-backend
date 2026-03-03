// Features/Cheques/Queries/ChequeQueries.cs
using MediatR;
using MotorStores.Application.DTOs;

namespace MotorStores.Application.Features.Cheques.Queries;

public record GetAllChequesQuery : IRequest<IEnumerable<ChequeDto>> { }

public record GetChequeByIdQuery : IRequest<ChequeReportDto?>
{
    public string Id { get; init; } = null!;
}

public record GetDueThisMonthQuery : IRequest<IEnumerable<ChequeReportDto>> { }

public record GetOverdueChequesQuery : IRequest<IEnumerable<ChequeReportDto>> { }

public record GetClearedChequesQuery : IRequest<IEnumerable<ChequeReportDto>> { }

public record GetStatusSummaryByBankAccountQuery : IRequest<IEnumerable<ChequeStatusSummaryDto>>
{
    public int BankAccountId { get; init; }
}

public record GetStatusSummaryByBankAccountTimeQuery : IRequest<IEnumerable<ChequeStatusSummaryDto>>
{
    public int BankAccountId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}
public record GetCurrentMonthTotalByAccountQuery : IRequest<decimal>
{
    public int BankAccountId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}