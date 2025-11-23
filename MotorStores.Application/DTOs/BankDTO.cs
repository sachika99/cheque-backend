namespace MotorStores.Application.DTOs
{
    public class BankDto
    {
        public int Id { get; set; }
        public string BankName { get; set; } = null!;
        public string BranchName { get; set; } = null!;
        public string BranchCode { get; set; } = null!;
        public string Status { get; set; } = "Active";
    }

    public class BankWithAccountsDto
    {
        public int Id { get; set; }
        public string BankName { get; set; } = null!;
        public string BranchName { get; set; } = null!;
        public string BranchCode { get; set; } = null!;
        public string Status { get; set; } = "Active";
        public List<BankAccountDto> BankAccounts { get; set; } = new();
    }
}
