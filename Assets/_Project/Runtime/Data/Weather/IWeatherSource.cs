using System.Threading;
using System.Threading.Tasks;

namespace Project.Data.Weather
{
    /// <summary>
    /// Предоставляет текущий прогноз погоды.
    /// </summary>
    public interface IWeatherSource
    {
        /// <summary>
        /// Возвращает прогноз или ошибку; позволяет вызывающему коду отменить запрос.
        /// </summary>
        /// <param name="timeoutSeconds">Положительный timeout операции источника в секундах; HTTP-адаптер применяет его к активному запросу.</param>
        /// <param name="cancellation">Отмена запроса.</param>
        Task<WeatherForecast> GetForecastAsync(int timeoutSeconds, CancellationToken cancellation);
    }
}