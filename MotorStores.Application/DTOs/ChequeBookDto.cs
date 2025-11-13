namespace MotorStores.Application.DTOs
{
    public class ChequeBookDto
    {
        public int Id { get; set; }
        public int BankAccountId { get; set; }
        public string AccountNo { get; set; } = null!;
        public string ChequeBookNo { get; set; } = null!;
        public int StartChequeNo { get; set; }
        public int EndChequeNo { get; set; }
        public int CurrentChequeNo { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime IssuedDate { get; set; }
        public int RemainingCheques => EndChequeNo - CurrentChequeNo;
    }
}
