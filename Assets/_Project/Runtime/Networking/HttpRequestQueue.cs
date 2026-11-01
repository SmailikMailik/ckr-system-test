using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Project.Common.Validation;
using Zenject;

[assembly: InternalsVisibleTo("Project.Runtime")]
[assembly: InternalsVisibleTo("Project.Data.Remote")]
[assembly: InternalsVisibleTo("Project.Tests.EditMode")]
[assembly: InternalsVisibleTo("Project.Tests.PlayMode")]

namespace Project.Networking
{
    /// <summary>
    /// Выполняет GET по порядку в основном цикле Unity и пропускает запросы с обнаруженной отменой.
    /// </summary>
    internal sealed class HttpRequestQueue : ITickable, IDisposable
    {
        private readonly IHttpTransport _transport;
        private readonly LinkedList<IRequest> _pending = new();
        private IRequest _active;
        private IHttpOperation _operation;
        private bool _disposed;

        /// <summary>
        /// Получает транспорт для запуска запросов, выбранных очередью.
        /// </summary>
        [Inject]
        internal HttpRequestQueue(IHttpTransport transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Добавляет запрос и возвращает задачу с его результатом; вызывается из основного потока Unity.
        /// При ближайшем Tick отменённый запрос удаляется из очереди, а активное соединение закрывается.
        /// </summary>
        /// <param name="url">Адрес ресурса.</param>
        /// <param name="timeoutSeconds">Положительный timeout активного HTTP-запроса в секундах.</param>
        /// <param name="acceptMediaType">Предпочтительный media type ответа, выбранный клиентом сервиса.</param>
        /// <param name="parse">Преобразование тела ответа в результат.</param>
        /// <param name="cancellation">Отмена ожидания и активной операции при ближайшем Tick.</param>
        internal Task<T> GetAsync<T>(
            string url,
            int timeoutSeconds,
            string acceptMediaType,
            Func<string, T> parse,
            CancellationToken cancellation)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(HttpRequestQueue));

            ArgumentGuard.ThrowIfNotPositive(timeoutSeconds, nameof(timeoutSeconds));
            ArgumentGuard.ThrowIfBlank(acceptMediaType, nameof(acceptMediaType));

            cancellation.ThrowIfCancellationRequested();
            var request = new Request<T>(url, timeoutSeconds, acceptMediaType, parse, cancellation);
            _pending.AddLast(request);
            return request.Task;
        }

        /// <summary>
        /// Завершает текущую операцию и запускает следующую только после освобождения предыдущей.
        /// </summary>
        public void Tick()
        {
            if (_disposed)
                return;

            RemoveCancelledRequests();
            CompleteActiveRequest();
            StartNextRequest();
        }

        /// <summary>
        /// Отменяет всю работу и освобождает активное соединение при завершении контекста.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _operation?.Dispose();
            _active?.Cancel();

            foreach (var request in _pending)
                request.Cancel();

            _pending.Clear();
            _operation = null;
            _active = null;
        }

        private void RemoveCancelledRequests()
        {
            for (var node = _pending.First; node is not null;)
            {
                var next = node.Next;

                if (node.Value.IsCancelled)
                {
                    node.Value.Cancel();
                    _pending.Remove(node);
                }

                node = next;
            }
        }

        private void CompleteActiveRequest()
        {
            if (_active is null)
                return;

            if (_active.IsCancelled is false && _operation.IsDone is false)
                return;

            var completed = _active;
            var operation = _operation;
            _active = null;
            _operation = null;
            // Сначала закрываем соединение, затем публикуем результат: продолжение задачи может добавить новый запрос.
            string response = null;
            Exception error = null;

            try
            {
                if (completed.IsCancelled is false)
                    response = operation.ReadResponse();
            }
            catch (Exception exception)
            {
                error = exception;
            }
            finally
            {
                operation.Dispose();
            }

            if (completed.IsCancelled)
                completed.Cancel();
            else if (error is not null)
                completed.Fail(error);
            else
                completed.Complete(response);
        }

        private void StartNextRequest()
        {
            while (_active is null && _pending.First is not null)
            {
                var request = _pending.First.Value;
                _pending.RemoveFirst();

                if (request.IsCancelled)
                {
                    request.Cancel();
                    continue;
                }

                try
                {
                    _operation = _transport.Get(request.Url, request.TimeoutSeconds, request.AcceptMediaType);
                    _active = request;
                }
                catch (Exception exception)
                {
                    request.Fail(exception);
                }
            }
        }

        /// <summary>
        /// Предоставляет последовательному планировщику параметры и операции завершения запроса.
        /// </summary>
        private interface IRequest
        {
            string Url { get; }
            int TimeoutSeconds { get; }
            string AcceptMediaType { get; }
            bool IsCancelled { get; }

            /// <summary>
            /// Преобразует успешный ответ в результат задачи потребителя.
            /// </summary>
            void Complete(string response);

            /// <summary>
            /// Завершает задачу ошибкой открытия, чтения или разбора ответа.
            /// </summary>
            void Fail(Exception error);

            /// <summary>
            /// Завершает задачу отменой без чтения и разбора ответа.
            /// </summary>
            void Cancel();
        }

        /// <summary>
        /// Связывает ответ транспорта, преобразование данных и задачу одного потребителя.
        /// </summary>
        private sealed class Request<T> : IRequest
        {
            private readonly Func<string, T> _parse;
            private readonly CancellationToken _cancellation;
            private readonly TaskCompletionSource<T> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

            /// <summary>
            /// Сохраняет параметры запроса и связывает его результат с отменой конкретного потребителя.
            /// </summary>
            internal Request(
                string url,
                int timeoutSeconds,
                string acceptMediaType,
                Func<string, T> parse,
                CancellationToken cancellation)
            {
                Url = url;
                TimeoutSeconds = timeoutSeconds;
                AcceptMediaType = acceptMediaType;
                _parse = parse;
                _cancellation = cancellation;
            }

            public string Url { get; }
            public int TimeoutSeconds { get; }
            public string AcceptMediaType { get; }
            public bool IsCancelled => _cancellation.IsCancellationRequested;
            internal Task<T> Task => _completion.Task;

            /// <summary>
            /// Публикует результат разбора или завершает задачу ошибкой парсера.
            /// </summary>
            public void Complete(string response)
            {
                try
                {
                    _completion.TrySetResult(_parse(response));
                }
                catch (Exception exception)
                {
                    Fail(exception);
                }
            }

            /// <summary>
            /// Завершает задачу текущего запроса указанной ошибкой.
            /// </summary>
            public void Fail(Exception error)
            {
                _completion.TrySetException(error);
            }

            /// <summary>
            /// Отменяет задачу при отмене потребителя или завершении всей очереди.
            /// </summary>
            public void Cancel()
            {
                _completion.TrySetCanceled();
            }
        }
    }
}