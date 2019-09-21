using Newtonsoft.Json;

namespace AccountApi.Models
{
    /// <summary>
    /// Ответ сервиса об истории транзакций по счету
    /// </summary>
    public class AccountHistoryResp : Response
    {
        /// <summary>
        /// История транзакций
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public AccountHistory[] AccountHistoryList { get; set; }
    }
}