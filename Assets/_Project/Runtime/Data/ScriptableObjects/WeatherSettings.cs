using Project.Data.Weather;
using UnityEngine;

namespace Project.Data.ScriptableObjects
{
    /// <summary>
    /// Хранит период опроса прогноза в сериализованном Unity asset.
    /// </summary>
    public sealed class WeatherSettings : ScriptableObject
    {
        [SerializeField, Min(0.1f)] private float _refreshSeconds = 5;

        /// <summary>
        /// Создаёт проверенный снимок периода опроса для приложения.
        /// </summary>
        public WeatherPollingConfig ToConfig()
        {
            return new(_refreshSeconds);
        }
    }
}