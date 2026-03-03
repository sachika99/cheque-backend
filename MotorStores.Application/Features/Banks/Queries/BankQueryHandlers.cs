// Features/Banks/Queries/BankQueryHandlers.cs
using MediatR;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Application.Mappings;

namespace MotorStores.Application.Features.Banks.Queries;

public class GetAllBanksQueryHandler : IRequestHandler<GetAllBanksQuery, IEnumerable<BankDto>>
{
    private readonly IBankRepository _bankRepository;

    public GetAllBanksQueryHandler(IBankRepository bankRepository)
        => _bankRepository = bankRepository;

    public async Task<IEnumerable<BankDto>> Handle(GetAllBanksQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var banks = await _bankRepository.GetAllAsync(cancellationToken);
            return banks.Select(BankMapper.ToDto);
        }
        catch (Exception ex)
        {
            throw new Exception($"Database error: {ex.Message}");
        }
    }
}

public class GetBankByIdQueryHandler : IRequestHandler<GetBankByIdQuery, BankDto?>
{
    private readonly IBankRepository _bankRepository;

    public GetBankByIdQueryHandler(IBankRepository bankRepository)
        => _bankRepository = bankRepository;

    public async Task<BankDto?> Handle(GetBankByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var bank = await _bankRepository.GetByIdAsync(request.Id, cancellationToken);
            return bank == null ? null : BankMapper.ToDto(bank);
        }
        catch (Exception ex)
        {
            throw new Exception($"Database error: {ex.Message}");
        }
    }
}