using System;
using UnityEngine.Networking;

namespace Project.Networking
{
    /// <summary>
    /// Сохраняет адрес, HTTP-код и результат завершившегося UnityWebRequest независимо от lifetime соединения.
    /// </summary>
    internal sealed class HttpRequestException : Exception
    {
        internal string Url { get; }
        internal long ResponseCode { get; }
        internal UnityWebRequest.Result Result { get; }

        /// <summary>
        /// Создаёт ошибку запроса с исходным сообщением транспорта и структурированными данными для диагностики.
        /// </summary>
        internal HttpRequestException(string url, long responseCode, UnityWebRequest.Result result, string error)
            : base($"GET {url} failed ({result}, HTTP {responseCode}): {error}")
        {
            Url = url;
            ResponseCode = responseCode;
            Result = result;
        }
    }
}