using System.Net;

namespace BosnetFinance.Models.ViewModels
{
    public class TransactionResponse
    {
        public object Data { get; set; }
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
    }
}