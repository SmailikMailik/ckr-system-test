namespace Project.Networking
{
    /// <summary>
    /// Запускает HTTP-запросы; их порядок и освобождение операций контролирует потребитель транспорта.
    /// </summary>
    internal interface IHttpTransport
    {
        /// <summary>
        /// Начинает GET-запрос; заданный timeout отсчитывается с момента запуска активной операции.
        /// </summary>
        /// <param name="url">Адрес запрашиваемого ресурса.</param>
        /// <param name="timeoutSeconds">Положительное время ожидания ответа в секундах.</param>
        /// <param name="acceptMediaType">Предпочтительный media type ответа для заголовка Accept.</param>
        IHttpOperation Get(string url, int timeoutSeconds, string acceptMediaType);
    }
}