using System.Configuration;

namespace AccountApi.Models
{
    /// <summary>
    /// Коды результата работы сервиса
    /// </summary>
    enum OperationCode
    {
        NOT_FOUND = 404,
        INVALID_REQUEST = 400,
        OK = 200,
        SERVICE_ERROR = 500
    };

    /// <summary>
    /// Конфигурация сервиса
    /// </summary>
    static public class Config
    {
        /// <summary>
        /// Строка подключения к SQL-серверу
        /// </summary>
        private static string _sqlConnStr;

        public static string SqlConnStr
        {
            get
            {
                if (string.IsNullOrEmpty(_sqlConnStr))
                    _sqlConnStr = ConfigurationManager.ConnectionStrings["Api"].ConnectionString;

                return _sqlConnStr;
            }
        }
    }
}