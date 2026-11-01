using Project.Common.Validation;
using UnityEngine.Networking;
using Zenject;

namespace Project.Networking
{
    /// <summary>
    /// Выполняет HTTP GET через UnityWebRequest с User-Agent, заданным при сборке приложения.
    /// </summary>
    internal sealed class UnityHttpTransport : IHttpTransport
    {
        private readonly string _userAgent;

        /// <summary>
        /// Получает идентификатор приложения для заголовка User-Agent.
        /// </summary>
        [Inject]
        internal UnityHttpTransport(string userAgent)
        {
            ArgumentGuard.ThrowIfBlank(userAgent, nameof(userAgent));

            _userAgent = userAgent;
        }

        /// <summary>
        /// Запускает GET с переданными Accept и timeout UnityWebRequest в секундах.
        /// </summary>
        public IHttpOperation Get(string url, int timeoutSeconds, string acceptMediaType)
        {
            ArgumentGuard.ThrowIfNotPositive(timeoutSeconds, nameof(timeoutSeconds));
            ArgumentGuard.ThrowIfBlank(acceptMediaType, nameof(acceptMediaType));

            return new Operation(url, timeoutSeconds, acceptMediaType, _userAgent);
        }

        /// <summary>
        /// Владеет одним UnityWebRequest; освобождение прерывает ещё не завершённый запрос.
        /// </summary>
        private sealed class Operation : IHttpOperation
        {
            private readonly UnityWebRequest _request;

            /// <summary>
            /// Создаёт и сразу отправляет GET; при ошибке настройки освобождает созданный запрос.
            /// </summary>
            internal Operation(
                string url,
                int timeoutSeconds,
                string acceptMediaType,
                string userAgent)
            {
                _request = UnityWebRequest.Get(url);

                try
                {
                    _request.timeout = timeoutSeconds;
                    _request.SetRequestHeader("Accept", acceptMediaType);
                    _request.SetRequestHeader("User-Agent", userAgent);
                    _request.SendWebRequest();
                }
                catch
                {
                    _request.Dispose();
                    throw;
                }
            }

            public bool IsDone => _request.isDone;

            /// <summary>
            /// Возвращает текст успешного ответа; ошибка HTTP или транспорта сохраняет URL, код и результат запроса.
            /// </summary>
            public string ReadResponse()
            {
                if (_request.result != UnityWebRequest.Result.Success)
                    throw new HttpRequestException(_request.url, _request.responseCode, _request.result, _request.error);

                return _request.downloadHandler.text;
            }

            /// <summary>
            /// Прерывает незавершённый запрос и освобождает его ресурсы.
            /// </summary>
            public void Dispose()
            {
                if (_request.isDone is false)
                    _request.Abort();

                _request.Dispose();
            }
        }
    }
}