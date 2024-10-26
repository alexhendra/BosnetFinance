using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BosnetFinance.Models.ViewModels
{
    public class TransactionRequest
    {
        public string AccountId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
    }
}