using Microsoft.EntityFrameworkCore;
using MotorStores.Application.Interfaces;
using MotorStores.Domain.Entities;
using MotorStores.Infrastructure.Persistence;

namespace MotorStores.Infrastructure.Repositories
{
    public class ChequeRepository : Repository<Cheque>, IChequeRepository
    {
        private readonly ApplicationDbContext _context;

        public ChequeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }


        public override async Task<IEnumerable<Cheque>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Cheques

                .Include(c => c.Invoices)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Cheque?> GetByChequeIdAsync(
            string chequeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Cheques
  
                .Include(c => c.Invoices)
                .FirstOrDefaultAsync(c => c.ChequeId == chequeId, cancellationToken);
        }
    }
}
