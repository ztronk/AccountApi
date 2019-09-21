using AccountApi.Models;
using AccountApi.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AccountApi.Controllers
{
    public class AccountController : ApiController
    {
        List<Account> accList = new List<Account>();
        List<AccountHistory> accHistList = new List<AccountHistory>();

        #region Конструктор
        /// <summary>
        /// Конструктор
        /// </summary>
        public AccountController()
        {
            accList.Add(
                new Account()
                {
                    Id = 1,
                    AccountNumber = "40818810000000001",
                    Balance = 1000
                });

            accList.Add(
                new Account()
                {
                    Id = 2,
                    AccountNumber = "40818810000000002",
                    Balance = 2000
                });

            accList.Add(
                new Account()
                {
                    Id = 3,
                    AccountNumber = "40818810000000003",
                    Balance = 3000
                });

            accHistList.Add(
                new AccountHistory()
                {
                    Id = 1,
                    AccId = 1,
                    Amount = 1500,
                    ChangedAt = DateTime.Now
                }
            );

            accHistList.Add(
                new AccountHistory()
                {
                    Id = 2,
                    AccId = 2,
                    Amount = 1700,
                    ChangedAt = DateTime.Now
                }
            );

            accHistList.Add(
                new AccountHistory()
                {
                    Id = 3,
                    AccId = 2,
                    Amount = 1200,
                    ChangedAt = DateTime.Now
                }
            );
        }
        #endregion

        #region Получение истории транзакций по счету
        /// <summary>
        /// Получение истории транзакций по счету
        /// </summary>
        /// <param name="accId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/account/{account_id:int}/history/")]
        public string GetAccountHistory([FromUri(Name = "account_id")]int? accId)
        {
            using (BaseQuery<AccountHistory> accHistQuery = new BaseQuery<AccountHistory>())
            {
                var accHistList = accHistQuery.GetList();
                return JsonConvert.SerializeObject(accHistList.FirstOrDefault());
            }
            //int _accId;
            //if (!accId.HasValue 
            //    || (accId.HasValue ? !int.TryParse(accId.Value.ToString(), out _accId) : false)
            //    || accId.Value < 1)
            //    return JsonConvert.SerializeObject(new AccountHistoryResp()
            //    {
            //        Code = (int)OperationCode.INVALID_REQUEST,
            //        Status = Enum.GetName(typeof(OperationCode), OperationCode.INVALID_REQUEST),
            //        Message = "Указан некорректный параметр запроса"
            //    });

            //var result = accHistList.Where(e => e.AccId == accId);

            //if (result == null)
            //    return JsonConvert.SerializeObject(new AccountHistoryResp()
            //    {
            //        Code = (int)OperationCode.SERVICE_ERROR,
            //        Status = Enum.GetName(typeof(OperationCode), OperationCode.SERVICE_ERROR),
            //        Message = "Внутренняя ошибка работы сервиса"
            //    });

            //if (!result?.Any() ?? false)
            //    return JsonConvert.SerializeObject(new AccountHistoryResp()
            //    {
            //        Code = (int)OperationCode.SERVICE_ERROR,
            //        Status = Enum.GetName(typeof(OperationCode), OperationCode.SERVICE_ERROR),
            //        Message = "Счет не найден"
            //    });

            //return JsonConvert.SerializeObject(new AccountHistoryResp()
            //{
            //    Code = (int)OperationCode.OK,
            //    Status = Enum.GetName(typeof(OperationCode), OperationCode.OK),
            //    Message = "Запрос выполнен успешно",
            //    AccountHistoryList = result.ToArray()
            //});
        }
        #endregion

        #region Внесение средств на счет
        /// <summary>
        /// Внесение средств на счет
        /// </summary>
        /// <param name="accId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/account/{account_id:int}/top-up")]
        public string TopUp([FromUri(Name = "account_id")]int? accId, decimal? amount)
        {
            int _accId;
            if (!accId.HasValue
                || (accId.HasValue ? !int.TryParse(accId.Value.ToString(), out _accId) : false)
                || accId.Value < 1)
                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.INVALID_REQUEST,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.INVALID_REQUEST),
                    Message = "Указан некорректный параметр запроса"
                });

            decimal _amount;
            if (!amount.HasValue
                || (amount.HasValue ? !decimal.TryParse(amount.Value.ToString(), out _amount) : false)
                || amount.Value <= 0)
                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.INVALID_REQUEST,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.INVALID_REQUEST),
                    Message = "Указан некорректный параметр запроса"
                });

            var result = accList.Where(e => e.Id == accId).FirstOrDefault();

            if (result == null)
                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.SERVICE_ERROR,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.SERVICE_ERROR),
                    Message = "Внутренняя ошибка работы сервиса"
                });

            decimal currBalance = result.Balance;

            try
            {
                result.Balance += amount.Value;

                accHistList.Add(new AccountHistory()
                {
                    Id = 4,
                    AccId = accId.Value,
                    ChangedAt = DateTime.Now,
                    Amount = result.Balance
                });

                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.OK,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.OK),
                    Message = $"Внесение средств на счет в сумме {amount.Value}. Баланс счета {result.Balance}"
                });
            }
            catch (Exception ex)
            {
                //Откат операции
                result.Balance = currBalance;

                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.SERVICE_ERROR,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.SERVICE_ERROR),
                    Message = $"Внутренняя ошибка работы сервиса. {ex.Message}"
                });
            }
        }
        #endregion

        #region Снятие средств со счета
        /// <summary>
        /// Снятие средств со счета
        /// </summary>
        /// <param name="accId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/account/{account_id:int}/withdraw")]
        public string WithDraw([FromUri(Name = "account_id")]int? accId, decimal? amount)
        {
            int _accId;
            if (!accId.HasValue
                || (accId.HasValue ? !int.TryParse(accId.Value.ToString(), out _accId) : false)
                || accId.Value < 1)
                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.NOT_FOUND,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.NOT_FOUND),
                    Message = "Указан некорректный параметр запроса"
                });

            decimal _amount;
            if (!amount.HasValue
                || (amount.HasValue ? !decimal.TryParse(amount.Value.ToString(), out _amount) : false)
                || amount.Value <= 0)
                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.NOT_FOUND,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.NOT_FOUND),
                    Message = "Указан некорректный параметр запроса"
                });

            var result = accList.Where(e => e.Id == accId).FirstOrDefault();

            if (result == null)
                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.SERVICE_ERROR,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.SERVICE_ERROR),
                    Message = "Внутренняя ошибка работы сервиса"
                });

            decimal currBalance = result.Balance;

            try
            {
                if (result.Balance - amount.Value < 0)
                    return JsonConvert.SerializeObject(new AccountHistoryResp()
                    {
                        Code = (int)OperationCode.OK,
                        Status = Enum.GetName(typeof(OperationCode), OperationCode.OK),
                        Message = $"Запрашиваемая для снятия сумма ({amount.Value}) превышает остаток на счете {result.Balance}"
                    });

                result.Balance -= amount.Value;

                accHistList.Add(new AccountHistory()
                {
                    Id = 5,
                    AccId = accId.Value,
                    ChangedAt = DateTime.Now,
                    Amount = result.Balance
                });

                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.OK,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.OK),
                    Message = $"Снятие средств со счета в сумме {amount.Value}. Баланс счета {result.Balance}"
                });
            }
            catch (Exception ex)
            {
                //Откат операции
                result.Balance = currBalance;

                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.SERVICE_ERROR,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.SERVICE_ERROR),
                    Message = $"Внутренняя ошибка работы сервиса. {ex.Message}"
                });
            }
        }
        #endregion

        #region Перевод денежных средств
        /// <summary>
        /// Перевод денежных средств
        /// </summary>
        /// <param name="srcAccId"></param>
        /// <param name="destAccId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/account/{source_account_id:int}/transfer/{destination_account_id:int}")]
        public string Transfer([FromUri(Name = "source_account_id")]int? srcAccId,
            [FromUri(Name = "destination_account_id")]int? destAccId,
            decimal? amount)
        {
            int _accId;
            if (!srcAccId.HasValue
                || (srcAccId.HasValue ? !int.TryParse(srcAccId.Value.ToString(), out _accId) : false)
                || srcAccId.Value < 1)
                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.NOT_FOUND,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.NOT_FOUND),
                    Message = "Указан некорректный параметр запроса"
                });

            if (!destAccId.HasValue
                || (destAccId.HasValue ? !int.TryParse(destAccId.Value.ToString(), out _accId) : false)
                || destAccId.Value < 1)
                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.NOT_FOUND,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.NOT_FOUND),
                    Message = "Указан некорректный параметр запроса"
                });

            decimal _amount;
            if (!amount.HasValue
                || (amount.HasValue ? !decimal.TryParse(amount.Value.ToString(), out _amount) : false)
                || amount.Value <= 0)
                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.NOT_FOUND,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.NOT_FOUND),
                    Message = "Указан некорректный параметр запроса"
                });

            var srcAcc = accList.Where(e => e.Id == srcAccId).FirstOrDefault();
            var descAcc = accList.Where(e => e.Id == destAccId).FirstOrDefault();

            if (srcAcc == null || descAcc == null)
                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.SERVICE_ERROR,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.SERVICE_ERROR),
                    Message = "Внутренняя ошибка работы сервиса"
                });

            decimal srcAccCurrBalance = srcAcc.Balance;
            decimal descAccCurrBalance = descAcc.Balance;

            try
            {
                if (srcAcc.Balance < amount.Value || srcAcc.Balance == 0)
                    return JsonConvert.SerializeObject(new AccountHistoryResp()
                    {
                        Code = (int)OperationCode.OK,
                        Status = Enum.GetName(typeof(OperationCode), OperationCode.OK),
                        Message = $"Запрашиваемая для снятия сумма ({amount.Value}) превышает остаток на счете {srcAcc.Balance}"
                    });

                srcAcc.Balance -= amount.Value;
                descAcc.Balance += amount.Value;

                accHistList.Add(new AccountHistory()
                {
                    Id = 5,
                    AccId = srcAcc.Id,
                    ChangedAt = DateTime.Now,
                    Amount = srcAcc.Balance
                });

                accHistList.Add(new AccountHistory()
                {
                    Id = 6,
                    AccId = descAcc.Id,
                    ChangedAt = DateTime.Now,
                    Amount = descAcc.Balance
                });

                return JsonConvert.SerializeObject(new TransferResp()
                {
                    Code = (int)OperationCode.OK,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.OK),
                    Message = $"Перевод средств со счета {srcAcc.Id} в сумме {amount.Value} на счет {descAcc.Id}.",
                    SourceBalance = srcAcc.Balance,
                    DestinationBalance = descAcc.Balance
                });
            }
            catch (Exception ex)
            {
                //Откат операции
                srcAcc.Balance = srcAccCurrBalance;
                descAcc.Balance = descAccCurrBalance;

                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.SERVICE_ERROR,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.SERVICE_ERROR),
                    Message = $"Внутренняя ошибка работы сервиса. {ex.Message}"
                });
            }
        }
        #endregion
    }
}
