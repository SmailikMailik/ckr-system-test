using System;
using Project.UI.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Views
{
    /// <summary>
    /// Отображает прогноз и состояние загрузки погоды.
    /// </summary>
    [DisallowMultipleComponent]
    internal sealed class WeatherView : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private WeatherThemeCatalog _themes;
        [SerializeField] private Image[] _cloudImages = Array.Empty<Image>();
        [SerializeField] private GameObject _stars;
        [SerializeField] private GameObject _rain;
        [SerializeField] private GameObject _snow;
        [SerializeField] private TMP_Text _temperatureLabel;
        [SerializeField] private TMP_Text _forecastLabel;
        [SerializeField] private Image _conditionIcon;
        [SerializeField] private TMP_Text _statusLabel;
        [SerializeField] private GameObject _loadingIndicator;

        /// <summary>
        /// Сбрасывает прогноз перед подключением экрана.
        /// </summary>
        internal void Initialize()
        {
            _temperatureLabel.text = "—";
            _forecastLabel.text = string.Empty;
            _conditionIcon.enabled = false;
        }

        /// <summary>
        /// Показывает серверный прогноз и локальный символ погодного состояния.
        /// </summary>
        internal void ShowWeather(double temperature, string unit, string forecast, Sprite icon, string themeCode = WeatherThemeCodes.Unknown)
        {
            _temperatureLabel.text = $"{temperature.ToString("0.#", UiFormatting.Culture)}°{unit}";
            _forecastLabel.text = $"Сегодня — {forecast}";
            _conditionIcon.sprite = icon;
            _conditionIcon.enabled = icon != null;

            var style = _themes != null ? _themes.GetStyle(themeCode) : null;

            if (_background != null && style is not null)
            {
                _background.sprite = style.Background;
                _background.color = Color.white;
            }

            foreach (var cloud in _cloudImages)
                cloud.color = style?.CloudTint ?? Color.white;

            if (_stars != null)
                _stars.SetActive(style?.Stars ?? false);

            if (_rain != null)
                _rain.SetActive(style?.Rain ?? false);

            if (_snow != null)
                _snow.SetActive(style?.Snow ?? false);
        }

        /// <summary>
        /// Отображает загрузку или результат обновления погоды.
        /// </summary>
        internal void ShowWeatherStatus(string status, bool loading)
        {
            _statusLabel.text = status;
            _statusLabel.gameObject.SetActive(string.IsNullOrEmpty(status) is false);
            _loadingIndicator.SetActive(loading);
        }

        /// <summary>
        /// Снимает индикатор при уходе с вкладки.
        /// </summary>
        internal void ClearLoading()
        {
            _loadingIndicator.SetActive(false);
        }
    }
}