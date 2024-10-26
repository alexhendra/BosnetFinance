using BosnetFinance.Models.ViewModels;
using BosnetFinance.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace BosnetFinance.Controllers
{
    [RoutePrefix("api/transaction")]
    public class TransactionController : ApiController
    {
        private readonly TransactionService _transactionService;

        public TransactionController()
        {
            _transactionService = new TransactionService();
        }

        [HttpPut]
        [Route("deposit")]
        public async Task<IHttpActionResult> Deposit([FromBody] TransactionRequest request)
        {
            var response = await _transactionService.SaveTransaction(request, Core.TransactionType.SETOR);
            return Ok(response);
        }

        [HttpPut]
        [Route("withdraw")]
        public async Task<IHttpActionResult> Withdraw([FromBody] TransactionRequest request)
        {
            var response = await _transactionService.SaveTransaction(request, Core.TransactionType.TARIK);
            return Ok(response);
        }

        [HttpPut]
        [Route("transfer")]
        public async Task<IHttpActionResult> Transfer([FromBody] TransactionRequest request)
        {
            var response = await _transactionService.SaveTransaction(request, Core.TransactionType.TRANSFER);
            return Ok(response);
        }

        [HttpGet]
        [Route("history")]
        public async Task<IHttpActionResult> GetHistory(string accountId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var history = await _transactionService.GetTransactionHistory(accountId, startDate, endDate);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
