using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BosnetFinance.Models.ViewModels
{
    public class TransactionResponse
    {
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}