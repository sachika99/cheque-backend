using MediatR;
using MotorStores.Application.DTOs;

namespace MotorStores.Application.Features.UserIds.Commands;

public class CreateUserIdCommand : IRequest<UserIdDto>
{
    public string UserId { get; set; }
    public string Role { get; set; }
    public string? CreatedBy { get; set; }
}

public class UpdateUserIdCommand : IRequest<UpdateUserIdDto>
{
    public int Id { get; set; }
    public string Role { get; set; }
    public string? CreatedBy { get; set; }
}

public class DeleteUserIdCommand : IRequest<bool>
{
    public int Id { get; set; }
}