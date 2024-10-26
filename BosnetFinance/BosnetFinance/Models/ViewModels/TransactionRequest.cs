using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BosnetFinance.Models.ViewModels
{
    public class TransactionRequest
    {
        [Required]
        public string AccountId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string CurrencyCode { get; set; }
    }
}