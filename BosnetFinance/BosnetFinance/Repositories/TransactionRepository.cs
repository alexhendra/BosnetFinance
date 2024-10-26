using BosnetFinance.Core;
using BosnetFinance.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BosnetFinance.Repositories
{
    public class TransactionRepository
    {
        private readonly string _connectionString;

        public TransactionRepository()
        {
            _connectionString = Configuration.ConnectionString;
        }

        public SqlConnection GetConnection() => new SqlConnection(_connectionString);

        public async Task<Int64> GetCounter()
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT TOP 1 iLastNumber FROM BOS_Counter WHERE szCounterId = '001-COU' ORDER BY iLastNumber DESC", connection);
                return (Int64)await command.ExecuteScalarAsync();
            }
        }

        public async Task UpdateCounter(SqlTransaction transaction)
        {
            var command = new SqlCommand(@"UPDATE BOS_Counter
                            SET iLastNumber = iLastNumber + 1                            
                            WHERE szCounterId = '001-COU'", transaction.Connection, transaction);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<decimal> GetAccountBalance(string accountId, string currencyId)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT decAmount FROM BOS_Balance WHERE szAccountId = @AccountId AND szCurrencyId = @CurrencyId", connection);
                command.Parameters.AddWithValue("@AccountId", accountId);
                command.Parameters.AddWithValue("@CurrencyId", currencyId);
                var result = await command.ExecuteScalarAsync();
                return result != null ? (decimal)result : 0;
            }
        }

        public async Task UpsertAccountBalance(string accountId, string currencyId, decimal amount, SqlTransaction transaction)
        {
            var checkCommand = new SqlCommand("SELECT COUNT(1) FROM BOS_Balance WHERE szAccountId = @AccountId AND szCurrencyId = @CurrencyId", transaction.Connection, transaction);
            checkCommand.Parameters.AddWithValue("@AccountId", accountId);
            checkCommand.Parameters.AddWithValue("@CurrencyId", currencyId);

            var exists = (int)checkCommand.ExecuteScalar() > 0;
            if (exists)
            {
                var command = new SqlCommand(@"UPDATE BOS_Balance
                            SET decAmount = decAmount + @Amount                            
                            WHERE szAccountId = @AccountId AND szCurrencyId = @CurrencyId", transaction.Connection, transaction);
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@AccountId", accountId);
                command.Parameters.AddWithValue("@CurrencyId", currencyId);
                await command.ExecuteNonQueryAsync();
            }
            else
            {
                var insertCommand = new SqlCommand("INSERT INTO BOS_Balance (szAccountId, szCurrencyId, decAmount) VALUES (@AccountId, @CurrencyId, @Amount)", transaction.Connection, transaction);
                insertCommand.Parameters.AddWithValue("@AccountId", accountId);
                insertCommand.Parameters.AddWithValue("@CurrencyId", currencyId);
                insertCommand.Parameters.AddWithValue("@Amount", amount);
                insertCommand.ExecuteNonQuery();
            }
        }

        public async Task SaveHistory(BOSHistory transaction, SqlTransaction sqlTransaction)
        {
            var command = new SqlCommand(
                "INSERT INTO BOS_History (szTransactionId, szAccountId, szCurrencyId, dtmTransaction, decAmount, szNote) VALUES (@TransactionId, @AccountId, @CurrencyId, @TransactionDate, @Amount, @TransactionType)",
                sqlTransaction.Connection, sqlTransaction);

            command.Parameters.AddWithValue("@TransactionId", transaction.TransactionId);
            command.Parameters.AddWithValue("@AccountId", transaction.AccountId);
            command.Parameters.AddWithValue("@CurrencyId", transaction.CurrencyId);
            command.Parameters.AddWithValue("@TransactionDate", transaction.TransactionDate);
            command.Parameters.AddWithValue("@Amount", transaction.Amount);
            command.Parameters.AddWithValue("@TransactionType", transaction.TransactionType);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<BOSHistory>> GetHistory(string accountId, DateTime startDate, DateTime endDate)
        {
            var transactions = new List<BOSHistory>();
            using (var connection = new SqlConnection(_connectionString))
            {
                // query to get transaction history based on account number and date range
                string sql = @"SELECT szTransactionId, szAccountId, szCurrencyId, dtmTransaction, decAmount, szNote 
                       FROM BOS_History 
                       WHERE szAccountId = @accountId AND dtmTransaction BETWEEN @StartDate AND @EndDate
                       ORDER BY dtmTransaction";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@accountId", accountId);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Read each transaction and add it to the list
                            transactions.Add(new BOSHistory
                            {
                                TransactionId = reader.GetString(0),
                                AccountId = reader.GetString(1),
                                CurrencyId = reader.GetString(2),
                                TransactionDate = reader.GetDateTime(3),
                                Amount = reader.GetDecimal(4),
                                TransactionType = reader.GetString(5)
                            });
                        }
                    }
                }
            }
            return transactions;
        }
    }
}