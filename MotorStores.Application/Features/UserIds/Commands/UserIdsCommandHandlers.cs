using MediatR;
using MotorStores.Application.DTOs;
using MotorStores.Application.Features.UserIds.Commands;
using MotorStores.Application.Interfaces;
using MotorStores.Application.Mappings;
using MotorStores.Domain.Entities;

namespace MotorStores.Application.Features.Invoices.Commands;

// ── CREATE ───────────────────────────────────────────────────────────────────
public class CreateUserIdUserIdCommandHandler : IRequestHandler<CreateUserIdCommand, UserIdDto>
{
    private readonly IUserIdRepository _userIdRepository;

    public CreateUserIdUserIdCommandHandler(IUserIdRepository userIdRepository)
    {
        _userIdRepository = userIdRepository;
    }

    public async Task<UserIdDto> Handle(CreateUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = new UserId
            {
                UserId = request.UserId,
                Role = request.Role,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _userIdRepository.AddAsync(entity, cancellationToken);
            return UserIdMapper.ToDto(created);
        }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

// ── UPDATE ───────────────────────────────────────────────────────────────────
public class UpdateUserIdCommandHandler : IRequestHandler<UpdateUserIdCommand, UpdateUserIdDto>
{
    private readonly IUserIdRepository _userIdRepository;

    public UpdateUserIdCommandHandler(IUserIdRepository userIdRepository)
    {
        _userIdRepository = userIdRepository;
    }

    public async Task<UpdateUserIdDto> Handle(UpdateUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await _userIdRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new Exception($"UserId with Id {request.Id} not found.");

         
            existing.Role = request.Role;

            var updated = await _userIdRepository.UpdateAsync(existing, cancellationToken);
            return UpdateUserIdMapper.ToDto(updated);
        }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}

// ── DELETE ───────────────────────────────────────────────────────────────────
public class DeleteUserIdCommandHandler : IRequestHandler<DeleteUserIdCommand, bool>
{
    private readonly IUserIdRepository _userIdRepository;

    public DeleteUserIdCommandHandler(IUserIdRepository userIdRepository)
    {
        _userIdRepository = userIdRepository;
    }

    public async Task<bool> Handle(DeleteUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await _userIdRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new Exception($"UserId with Id {request.Id} not found.");

            return await _userIdRepository.DeleteAsync(request.Id, cancellationToken);
        }
        catch (Exception ex) { throw new Exception($"Database error: {ex.Message}"); }
    }
}