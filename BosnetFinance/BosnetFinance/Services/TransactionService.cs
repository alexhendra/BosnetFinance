using BosnetFinance.Core;
using BosnetFinance.Models.Entities;
using BosnetFinance.Models.ViewModels;
using BosnetFinance.Repositories;
using System;
using System.Data;
using System.Net;
using System.Threading.Tasks;

namespace BosnetFinance.Services
{
    public class TransactionService
    {
        private readonly TransactionRepository _repository;

        public TransactionService()
        {
            _repository = new TransactionRepository();
        }

        private async Task<string> GenerateTransactionId()
        {
            Int64 counterId = await _repository.GetCounter();
            decimal counterIdTemp = counterId / 100000.0m;
            return $"{DateTime.Now:yyyyMMdd}-{string.Format("{0:00000.00000}", counterIdTemp)}";
        }

        public async Task<TransactionResponse> SaveTransaction(TransactionRequest request, TransactionType transactionType)
        {
            if (request == null || request.Amount <= 0)
                return new TransactionResponse { Data = null, Status = HttpStatusCode.BadRequest, Message = "Invalid payload data!" };

            if (transactionType == TransactionType.TARIK || transactionType == TransactionType.TRANSFER)
            {
                decimal balanceAmount = await _repository.GetAccountBalance(request.AccountId, request.CurrencyCode);
                if (balanceAmount <= 0 || (balanceAmount - request.Amount) < 0)
                    return new TransactionResponse
                    {
                        Data = null,
                        Status = HttpStatusCode.BadRequest,
                        Message = "Your balance is insufficient to perform this transaction!"
                    };

                request.Amount *= -1;
            }

            string transactionId = await GenerateTransactionId();
            var transaction = new BOSHistory
            {
                TransactionId = transactionId,
                AccountId = request.AccountId,
                Amount = request.Amount,
                TransactionType = transactionType.ToString(),
                CurrencyId = request.CurrencyCode,
                TransactionDate = DateTime.UtcNow
            };

            using (var connection = _repository.GetConnection())
            {
                await connection.OpenAsync();
                using (var sqlTransaction = connection.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        await _repository.UpsertAccountBalance(request.AccountId, request.CurrencyCode, request.Amount, sqlTransaction);
                        await _repository.SaveHistory(transaction, sqlTransaction);
                        await _repository.UpdateCounter(sqlTransaction);
                        sqlTransaction.Commit();
                        return new TransactionResponse { Data = new { TransactionId = transactionId }, Status = HttpStatusCode.Created, Message = "Transaction successfully completed." };
                    }
                    catch
                    {
                        sqlTransaction.Rollback();
                        return new TransactionResponse { Data = null, Status = HttpStatusCode.InternalServerError, Message = "Something when wrong!" };
                    }
                }
            }
        }

        public async Task<TransactionResponse> GetTransactionHistory(string accountId, DateTime startDate, DateTime endDate)
        {
            var result = await _repository.GetHistory(accountId, startDate, endDate);
            return new TransactionResponse { Data = result, Status = result.Count > 0 ? HttpStatusCode.OK : HttpStatusCode.NotFound, Message = result.Count > 0 ? "Data found!" : "Data not found!" };
        }
    }
}