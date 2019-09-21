using AccountApi.Models;
using AccountApi.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountApi.Query
{
    public class AccountHistoryQuery : BaseQuery<AccountHistory>
    {
        /// <summary>
        /// Получение истории транзакции по счету
        /// </summary>
        /// <param name="accId"></param>
        /// <returns></returns>
        public IEnumerable<AccountHistory> GetAccountHistory(int accId)
        {
            return _context
                    .Where(e => e.AccId.Equals(accId))
                    .OrderByDescending(s => s.Id);
        }
    }
}