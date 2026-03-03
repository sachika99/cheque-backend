// Infrastructure/Repositories/InvoiceRepository.cs
using Microsoft.EntityFrameworkCore;
using MotorStores.Application.Interfaces;
using MotorStores.Domain.Entities;
using MotorStores.Infrastructure.Persistence;

namespace MotorStores.Infrastructure.Repositories;

public class UserIdRepository : Repository<UserId>, IUserIdRepository
{
    public UserIdRepository(ApplicationDbContext context)
        : base(context)
    { }



    public async Task<UserId?> GetAllByUserIdAsync(
         string userId,
         CancellationToken cancellationToken = default)
         => await _dbSet
             .FirstOrDefaultAsync(i => i.UserId == userId, cancellationToken);
}