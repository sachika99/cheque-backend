namespace MotorStores.Application.DTOs
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string? InvoiceNo { get; set; }
        public decimal? InvoiceAmount { get; set; }

        public int? ChequeId { get; set; }
    }
}
