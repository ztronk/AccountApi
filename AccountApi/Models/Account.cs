using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AccountApi.Models
{
    /// <summary>
    /// Банковский счет
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Идентификатор счета
        /// </summary>
        [Key]
        [JsonProperty(PropertyName = "account_id")]
        public int Id { get; set; }

        /// <summary>
        /// Номер счета
        /// </summary>
        [JsonProperty(PropertyName = "account_number")]
        public string AccountNumber { get; set; }

        /// <summary>
        /// Остаток на счете
        /// </summary>
        [JsonProperty(PropertyName = "balance")]
        public decimal Balance { get; set; }
    }
}