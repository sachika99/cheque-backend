// Application/Mappings/InvoiceMapper.cs
using MotorStores.Application.DTOs;
using MotorStores.Domain.Entities;

namespace MotorStores.Application.Mappings;

public static class InvoiceMapper
{
    public static InvoiceDto ToDto(Invoice invoice) => new InvoiceDto
    {
        Id = invoice.Id,
        InvoiceNo = invoice.InvoiceNo,
        InvoiceAmount = invoice.InvoiceAmount
    };
}