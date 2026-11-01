using System;

namespace Project.Networking
{
    /// <summary>
    /// Сообщает о завершении запроса и предоставляет ответ; Dispose прерывает незавершённую операцию.
    /// </summary>
    /// <remarks>
    /// <see cref="IsDone"/> сообщает о завершении операции, в том числе с ошибкой или отменой.
    /// </remarks>
    internal interface IHttpOperation : IDisposable
    {
        bool IsDone { get; }

        /// <summary>
        /// Читает тело успешного завершённого ответа или сообщает ошибку; вызывается после IsDone.
        /// </summary>
        string ReadResponse();
    }
}