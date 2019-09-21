using Newtonsoft.Json;

namespace AccountApi.Models
{
    /// <summary>
    /// Перевод денежных средств
    /// </summary>
    public class TransferResp : Response
    {
        /// <summary>
        /// Текущий баланс исходного счета
        /// </summary>
        [JsonProperty(PropertyName = "source_balance")]
        public decimal SourceBalance { get; set; }

        /// <summary>
        /// Текущий баланс целевого счета
        /// </summary>
        [JsonProperty(PropertyName = "destination_balance")]
        public decimal DestinationBalance { get; set; }
    }
}