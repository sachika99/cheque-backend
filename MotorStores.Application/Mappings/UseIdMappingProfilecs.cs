// Application/Mappings/InvoiceMapper.cs
using MotorStores.Application.DTOs;
using MotorStores.Domain.Entities;

namespace MotorStores.Application.Mappings;

public static class UserIdMapper
{
    public static UserIdDto ToDto(UserId userId) => new UserIdDto
    {
        Id = userId.Id,
        UserId = userId.UserId,
        Role = userId.Role,
        CreatedBy = userId.CreatedBy,
    };
}

public static class UpdateUserIdMapper
{
    public static UpdateUserIdDto ToDto(UserId userId) => new UpdateUserIdDto
    {
        Role = userId.Role,
    };
}