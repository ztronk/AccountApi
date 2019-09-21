using AccountApi.Models;
using AccountApi.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountApi.Query
{
    public class AccountQuery : BaseQuery<Account>
    {
        /// <summary>
        /// Получение истории транзакции по счету
        /// </summary>
        /// <param name="accId"></param>
        /// <returns></returns>
        public Account GetAccount(int accId)
        {
            return _context
                    .Where(e => e.Id.Equals(accId))
                    .FirstOrDefault();
        }
    }
}