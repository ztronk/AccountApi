using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AccountApi.Models
{
    /// <summary>
    /// История транзакций по счету
    /// </summary>
    public class AccountHistory
    {
        /// <summary>
        /// Идентификатор транзакции
        /// </summary>
        [Key]
        [JsonProperty(PropertyName = "history_id")]
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор счета
        /// </summary>
        [JsonProperty(PropertyName = "account_id")]
        public int AccId { get; set; }

        /// <summary>
        /// Сумма транзакции
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Дата транзакции
        /// </summary>
        [JsonProperty(PropertyName = "changed_at")]
        public DateTime ChangedAt { get; set; }
    }
}