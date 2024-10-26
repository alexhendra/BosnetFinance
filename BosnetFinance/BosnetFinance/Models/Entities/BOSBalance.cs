using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BosnetFinance.Models.Entities
{
    public class BOSBalance
    {
        public string AccountId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyId { get; set; }
    }
}