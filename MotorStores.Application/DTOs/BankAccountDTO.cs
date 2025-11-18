using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorStores.Application.DTOs
{
    public class BankAccountDto
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; } = null!;
        public string AccountNo { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public string AccountType { get; set; } = null!;
        public decimal Balance { get; set; }
        public string Status { get; set; } = "Active";
    }
}
