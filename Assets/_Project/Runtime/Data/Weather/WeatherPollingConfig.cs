using Project.Common.Validation;

namespace Project.Data.Weather
{
    /// <summary>
    /// Хранит проверенный неизменяемый период опроса прогноза.
    /// </summary>
    public sealed class WeatherPollingConfig
    {
        /// <summary>
        /// Интервал между запросами прогноза в секундах.
        /// </summary>
        public float RefreshSeconds { get; }

        /// <summary>
        /// Проверяет и сохраняет конечный положительный период опроса.
        /// </summary>
        public WeatherPollingConfig(float refreshSeconds)
        {
            ArgumentGuard.ThrowIfNotPositiveFinite(refreshSeconds, nameof(refreshSeconds));

            RefreshSeconds = refreshSeconds;
        }
    }
}