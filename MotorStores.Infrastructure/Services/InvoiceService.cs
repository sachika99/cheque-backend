using Microsoft.EntityFrameworkCore;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Domain.Entities;
using MotorStores.Infrastructure.Persistence;

namespace MotorStores.Infrastructure.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;

        public InvoiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllAsync()
        {
            var invoices = await _context.Invoices
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return invoices.Select(MapToDto);
        }

        public async Task<InvoiceDto?> GetByIdAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            return invoice == null ? null : MapToDto(invoice);
        }

        public async Task<InvoiceDto> CreateAsync(InvoiceDto dto)
        {
            var invoice = new Invoice
            {
                InvoiceNo = dto.InvoiceNo,
                InvoiceAmount = dto.InvoiceAmount,
                CreatedAt = DateTime.UtcNow
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return MapToDto(invoice);
        }

        public async Task<InvoiceDto> UpdateAsync(InvoiceDto dto)
        {
            var invoice = await _context.Invoices.FindAsync(dto.Id);

            if (invoice == null)
                throw new InvalidOperationException($"Invoice with ID {dto.Id} not found.");

            invoice.InvoiceNo = dto.InvoiceNo;
            invoice.InvoiceAmount = dto.InvoiceAmount;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(invoice);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
                return false;

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return true;
        }

        private static InvoiceDto MapToDto(Invoice invoice)
        {
            return new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceNo = invoice.InvoiceNo,
                InvoiceAmount = invoice.InvoiceAmount
            };
        }
    }
}
