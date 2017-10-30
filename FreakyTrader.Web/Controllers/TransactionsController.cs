using System;
using System.Collections.Generic;
using FreakyTrader.Data.Context;
using FreakyTrader.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FreakyTrader.Web.Controllers
{
    [Route("api/[controller]")]
    public class TransactionsController : Controller
    {
        private TransactionRepository _transactionRepo;

        public TransactionsController(TransactionRepository transactionRepo)
        {
            _transactionRepo = transactionRepo;
        }

        [HttpGet("")]
        public IActionResult GetTransactions(string code, DateTime startDate, DateTime endDate, int period)
        {
            var result = _transactionRepo.GetCandles(code, startDate, endDate, period);

            return Ok(new
            {
                Transactions = result
            });
        }
    }
}
