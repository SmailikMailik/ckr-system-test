using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Project.Data.Weather;
using Project.Networking;
using Zenject;

[assembly: InternalsVisibleTo("Project.Runtime")]
[assembly: InternalsVisibleTo("Project.Tests.EditMode")]

namespace Project.Data.Remote.Nws
{
    /// <summary>
    /// Получает прогноз сетки NWS в GeoJSON и передаёт ответ сервисному парсеру.
    /// </summary>
    internal sealed class NwsWeatherApi : IWeatherSource
    {
        private const string Endpoint = "https://api.weather.gov/gridpoints";
        private const string AcceptMediaType = "application/geo+json";
        private readonly string _forecastUrl;
        private readonly HttpRequestQueue _queue;

        /// <summary>
        /// Получает очередь приложения и точку сетки NWS и формирует адрес прогноза.
        /// </summary>
        [Inject]
        internal NwsWeatherApi(HttpRequestQueue queue, NwsGridpoint gridpoint)
        {
            _queue = queue;
            _forecastUrl = FormattableString.Invariant(
                $"{Endpoint}/{gridpoint.OfficeId}/{gridpoint.X},{gridpoint.Y}/forecast");
        }

        /// <summary>
        /// Запрашивает GeoJSON NWS с заданным timeout и возвращает первый период ответа.
        /// </summary>
        public Task<WeatherForecast> GetForecastAsync(int timeoutSeconds, CancellationToken cancellation)
        {
            return _queue.GetAsync(_forecastUrl, timeoutSeconds, AcceptMediaType, NwsForecastParser.Parse, cancellation);
        }
    }
}