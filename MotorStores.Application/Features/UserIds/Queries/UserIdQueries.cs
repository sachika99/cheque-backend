using MediatR;
using MotorStores.Application.DTOs;

namespace MotorStores.Application.Features.UserIds.Queries;

public record GetIdByUserQuery : IRequest<UserIdDto?>
{
    public string UserId { get; init; }
}

public record GetUserByIdQuery : IRequest<UserIdDto?>
{
    public int Id { get; init; }
}

public record GetAllUserIdsQuery : IRequest<IEnumerable<UserIdDto>>;
