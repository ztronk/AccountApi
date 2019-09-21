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

            AccountHistory[] accHistList;

            using (AccountHistoryQuery accHistQuery = new AccountHistoryQuery())
                accHistList = accHistQuery.GetAccountHistory(accId.Value).ToArray();


            if (accHistList == null)
                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.SERVICE_ERROR,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.SERVICE_ERROR),
                    Message = "Внутренняя ошибка работы сервиса"
                });

            if (!accHistList.Any())
                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.SERVICE_ERROR,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.NOT_FOUND),
                    Message = "Счет для проведения транзакции не найден"
                });

            using (AccountHistoryQuery accHistQuery = new AccountHistoryQuery())
            {
                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.OK,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.OK),
                    Message = "Запрос выполнен успешно",
                    AccountHistoryList = accHistQuery.GetAccountHistory(accId.Value).ToArray()
                });
            }
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

            //Получение информации о счете
            Account acc = new Account();
            using (AccountQuery accQuery = new AccountQuery())
                acc = accQuery.GetAccount(accId.Value);

            if (acc == null)
                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.SERVICE_ERROR,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.NOT_FOUND),
                    Message = "Счет для проведения транзакции не найден"
                });

            decimal currBalance = acc.Balance;

            try
            {
                acc.Balance += amount.Value;

                using (AccountQuery accQuery = new AccountQuery())
                    accQuery.UpdateEntity(acc);

                using (AccountHistoryQuery accHistQuery = new AccountHistoryQuery())
                    accHistQuery.CreateEntity(new AccountHistory()
                    {
                        AccId = acc.Id,
                        ChangedAt = DateTime.Now,
                        Amount = acc.Balance
                    });

                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.OK,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.OK),
                    Message = $"Внесение средств на счет в сумме {amount.Value}. Баланс счета {acc.Balance}"
                });
            }
            catch (Exception ex)
            {
                //Откат операции
                using (AccountQuery accQuery = new AccountQuery())
                {
                    acc.Balance = currBalance;
                    accQuery.UpdateEntity(acc);
                }
                    
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

            //Получение информации о счете
            Account acc = new Account();
            using (AccountQuery accQuery = new AccountQuery())
                acc = accQuery.GetAccount(accId.Value);

            if (acc == null)
                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.SERVICE_ERROR,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.NOT_FOUND),
                    Message = "Счет для проведения транзакции не найден"
                });

            decimal currBalance = acc.Balance;

            try
            {
                if (acc.Balance - amount.Value < 0)
                    return JsonConvert.SerializeObject(new AccountHistoryResp()
                    {
                        Code = (int)OperationCode.OK,
                        Status = Enum.GetName(typeof(OperationCode), OperationCode.OK),
                        Message = $"Запрашиваемая для снятия сумма ({amount.Value}) превышает остаток на счете {acc.Balance}"
                    });

                acc.Balance -= amount.Value;

                using (AccountQuery accQuery = new AccountQuery())
                    accQuery.UpdateEntity(acc);

                using (AccountHistoryQuery accHistQuery = new AccountHistoryQuery())
                    accHistQuery.CreateEntity(new AccountHistory()
                    {
                        AccId = acc.Id,
                        ChangedAt = DateTime.Now,
                        Amount = acc.Balance
                    });

                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.OK,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.OK),
                    Message = $"Снятие средств со счета в сумме {amount.Value}. Баланс счета {acc.Balance}"
                });
            }
            catch (Exception ex)
            {
                //Откат операции
                using (AccountQuery accQuery = new AccountQuery())
                {
                    acc.Balance = currBalance;
                    accQuery.UpdateEntity(acc);
                }

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

            //Получение информации о счетах
            Account srcAcc, descAcc = new Account();
            using (AccountQuery accQuery = new AccountQuery())
            {
                srcAcc = accQuery.GetAccount(srcAccId.Value);
                descAcc = accQuery.GetAccount(destAccId.Value);
            }

            if (srcAcc == null || descAcc == null)
                return JsonConvert.SerializeObject(new AccountHistoryResp()
                {
                    Code = (int)OperationCode.SERVICE_ERROR,
                    Status = Enum.GetName(typeof(OperationCode), OperationCode.NOT_FOUND),
                    Message = "Счет для проведения транзакции не найден"
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

                using (AccountQuery accQuery = new AccountQuery())
                {
                    accQuery.UpdateEntity(srcAcc);
                    accQuery.UpdateEntity(descAcc);
                }

                using (AccountHistoryQuery accHistQuery = new AccountHistoryQuery())
                {
                    accHistQuery.CreateEntity(new AccountHistory()
                    {
                        AccId = srcAcc.Id,
                        ChangedAt = DateTime.Now,
                        Amount = srcAcc.Balance
                    });

                    accHistQuery.CreateEntity(new AccountHistory()
                    {
                        AccId = descAcc.Id,
                        ChangedAt = DateTime.Now,
                        Amount = descAcc.Balance
                    });
                }

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
                using (AccountQuery accQuery = new AccountQuery())
                {
                    srcAcc.Balance = srcAccCurrBalance;
                    descAcc.Balance = descAccCurrBalance;

                    accQuery.UpdateEntity(srcAcc);
                    accQuery.UpdateEntity(descAcc);
                }

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
