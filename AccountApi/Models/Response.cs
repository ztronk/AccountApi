using Newtonsoft.Json;

namespace AccountApi.Models
{
    /// <summary>
    /// Ответ сервиса
    /// </summary>
    public abstract class Response
    {
        /// <summary>
        /// Код ответа сервиса
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}