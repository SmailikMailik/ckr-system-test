using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Project.Common.Validation;
using Project.Data.Weather;
using Project.UI.Settings;
using Project.UI.Views;
using UnityEngine;
using Zenject;

[assembly: InternalsVisibleTo("Project.Runtime")]
[assembly: InternalsVisibleTo("Project.Tests.PlayMode")]
[assembly: InternalsVisibleTo("Project.Tests.EditMode")]

namespace Project.Features.Weather
{
    /// <summary>
    /// Обновляет прогноз только на активной вкладке и не допускает отображения ответов после ухода с неё.
    /// </summary>
    internal sealed class WeatherTabPresenter
    {
        private readonly WeatherView _view;
        private readonly IWeatherSource _weather;
        private readonly WeatherPollingConfig _settings;
        private readonly WeatherIconCatalog _icons;
        private readonly int _timeoutSeconds;
        private CancellationTokenSource _cancellation;
        private double _elapsed;
        private int _requests;

        /// <summary>
        /// Получает представление, источник прогноза и политику опроса погодной вкладки.
        /// </summary>
        [Inject]
        internal WeatherTabPresenter(
            WeatherView view,
            IWeatherSource weather,
            WeatherPollingConfig settings,
            WeatherIconCatalog icons,
            int timeoutSeconds)
        {
            _view = view;
            _weather = weather;
            _settings = settings;
            _icons = icons;
            _timeoutSeconds = timeoutSeconds;
        }

        /// <summary>
        /// Начинает новый период активности вкладки с немедленного запроса прогноза.
        /// </summary>
        internal void Enter()
        {
            Exit();
            _cancellation = new();
            _elapsed = 0;
            _requests = 0;
            _ = LoadAsync(_cancellation.Token);
        }

        /// <summary>
        /// Планирует опрос по интервалу; пропущенные внутри одного шага границы объединяет в один запрос.
        /// </summary>
        internal void Tick(float elapsed)
        {
            ArgumentGuard.ThrowIfNegativeOrNotFinite(elapsed, nameof(elapsed));

            if (_cancellation is null)
                return;

            _elapsed += elapsed;

            if (_elapsed < _settings.RefreshSeconds)
                return;

            // Старые интервалы не несут новых данных: один кадр не должен создавать неограниченную очередь.
            _elapsed %= _settings.RefreshSeconds;
            _ = LoadAsync(_cancellation.Token);
        }

        /// <summary>
        /// Отменяет активные и ожидающие запросы; поздние ответы больше не меняют интерфейс.
        /// </summary>
        internal void Exit()
        {
            if (_cancellation is null)
                return;

            _cancellation.Cancel();
            _cancellation.Dispose();
            _cancellation = null;
        }

        private async Task LoadAsync(CancellationToken cancellation)
        {
            _requests++;
            _view.ShowWeatherStatus("Загрузка прогноза…", true);

            try
            {
                var forecast = await _weather.GetForecastAsync(_timeoutSeconds, cancellation);

                if (cancellation.IsCancellationRequested)
                    return;

                var appearance = WeatherPresentationMapper.Map(forecast.Condition, forecast.IsDaytime);
                _view.ShowWeather(
                    forecast.Temperature,
                    forecast.Unit,
                    forecast.Description,
                    _icons.Get(appearance.IconCode),
                    appearance.ThemeCode);
                _view.ShowWeatherStatus(string.Empty, _requests > 1);
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
            {
                // Отменённый запрос больше не должен обновлять представление.
            }
            catch (Exception exception)
            {
                if (cancellation.IsCancellationRequested is false)
                {
                    _view.ShowWeatherStatus("Не удалось загрузить прогноз. Повторим автоматически.", _requests > 1);
                    Debug.LogWarning(
                        $"[{nameof(WeatherTabPresenter)}::{nameof(LoadAsync)}] Weather refresh failed: {exception}",
                        _view);
                }
            }
            finally
            {
                if (cancellation.IsCancellationRequested is false)
                    _requests--;
            }
        }
    }
}