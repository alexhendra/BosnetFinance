using BosnetFinance.Models.ViewModels;
using BosnetFinance.Services;
using System;
using System.Net;
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

            var response = new TransactionResponse
            {
                Data = null,
                Status = HttpStatusCode.BadRequest,
                Message = "Invalid payload data!"
            };

            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, response);

            try
            {
                response = await _transactionService.SaveTransaction(request, Core.TransactionType.SETOR);
                return Content(response.Status, response);
            }
            catch (Exception)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.Message = "Something when wrong!";
                return Content(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPut]
        [Route("withdraw")]
        public async Task<IHttpActionResult> Withdraw([FromBody] TransactionRequest request)
        {
            var response = new TransactionResponse
            {
                Data = null,
                Status = HttpStatusCode.BadRequest,
                Message = "Invalid payload data!"
            };

            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, response);

            try
            {
                response = await _transactionService.SaveTransaction(request, Core.TransactionType.TARIK);
                return Content(response.Status, response);
            }
            catch (Exception)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.Message = "Something when wrong!";
                return Content(HttpStatusCode.InternalServerError, response);
            }

        }

        [HttpPut]
        [Route("transfer")]
        public async Task<IHttpActionResult> Transfer([FromBody] TransactionRequest request)
        {
            var response = new TransactionResponse
            {
                Data = null,
                Status = HttpStatusCode.BadRequest,
                Message = "Invalid payload data!"
            };

            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, response);

            try
            {
                response = await _transactionService.SaveTransaction(request, Core.TransactionType.TRANSFER);
                return Content(response.Status, response);
            }
            catch (Exception)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.Message = "Something when wrong!";
                return Content(HttpStatusCode.InternalServerError, response);
            }

        }

        [HttpGet]
        [Route("history")]
        public async Task<IHttpActionResult> GetHistory(string accountId, DateTime startDate, DateTime endDate)
        {
            var response = new TransactionResponse
            {
                Data = null,
                Status = HttpStatusCode.InternalServerError,
                Message = "Something when wrong!"
            };

            try
            {
                response = await _transactionService.GetTransactionHistory(accountId, startDate, endDate);
                return Content(response.Status, response);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.InternalServerError, response);
            }
        }
    }
}
