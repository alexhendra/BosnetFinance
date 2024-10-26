using System;

namespace BosnetFinance.Models.Entities
{
    public class BOSHistory
    {
        public string TransactionId { get; set; }
        public string AccountId { get; set; }
        public string CurrencyId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }   
    }
}