using MediatR;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Application.Mappings;

namespace MotorStores.Application.Features.UserIds.Queries;

// ── GET BY USER ID ────────────────────────────────────────────────────────────
public class GetIdByUserQueryHandler : IRequestHandler<GetIdByUserQuery, UserIdDto?>
{
    private readonly IUserIdRepository _userIdRepository;

    public GetIdByUserQueryHandler(IUserIdRepository userIdRepository)
        => _userIdRepository = userIdRepository;

    public async Task<UserIdDto?> Handle(GetIdByUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await _userIdRepository.GetAllByUserIdAsync(request.UserId, cancellationToken);
            return userId == null ? null : UserIdMapper.ToDto(userId);
        }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

// ── GET BY INT ID ─────────────────────────────────────────────────────────────
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserIdDto?>
{
    private readonly IUserIdRepository _userIdRepository;

    public GetUserByIdQueryHandler(IUserIdRepository userIdRepository)
        => _userIdRepository = userIdRepository;

    public async Task<UserIdDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await _userIdRepository.GetByIdAsync(request.Id, cancellationToken);
            return userId == null ? null : UserIdMapper.ToDto(userId);
        }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

// ── GET ALL ───────────────────────────────────────────────────────────────────
public class GetAllUserIdsQueryHandler : IRequestHandler<GetAllUserIdsQuery, IEnumerable<UserIdDto>>
{
    private readonly IUserIdRepository _userIdRepository;

    public GetAllUserIdsQueryHandler(IUserIdRepository userIdRepository)
        => _userIdRepository = userIdRepository;

    public async Task<IEnumerable<UserIdDto>> Handle(GetAllUserIdsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userIds = await _userIdRepository.GetAllAsync(cancellationToken);
            return userIds.Select(UserIdMapper.ToDto);
        }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}