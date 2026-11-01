using System.Collections.Generic;
using Project.Data.Weather;
using Icons = Project.UI.Settings.WeatherIconCodes;
using Themes = Project.UI.Settings.WeatherThemeCodes;

namespace Project.Features.Weather
{
    /// <summary>
    /// Переводит погодное явление и время суток в локальные коды оформления на границе Data и UI.
    /// </summary>
    internal static class WeatherPresentationMapper
    {
        private static readonly Appearance _unknown = new(Icons.UnknownWeather, Themes.Unknown);
        private static readonly Dictionary<WeatherCondition, Appearance> _dayAppearances = new()
        {
            [WeatherCondition.Clear] = new(Icons.ClearDay, Themes.Day),
            [WeatherCondition.PartlyCloudy] = new(Icons.PartlyCloudyDay, Themes.Day),
            [WeatherCondition.MostlyCloudy] = new(Icons.MostlyCloudyDay, Themes.Cloud),
            [WeatherCondition.Overcast] = new(Icons.Cloudy, Themes.Cloud),
            [WeatherCondition.Windy] = new(Icons.Wind, Themes.Day),
            [WeatherCondition.Snow] = new(Icons.Snow, Themes.Snow),
            [WeatherCondition.RainSnow] = new(Icons.RainSnow, Themes.Snow),
            [WeatherCondition.RainSleet] = new(Icons.RainSleet, Themes.Cold),
            [WeatherCondition.SnowSleet] = new(Icons.SnowSleet, Themes.Cold),
            [WeatherCondition.FreezingRain] = new(Icons.FreezingRain, Themes.Cold),
            [WeatherCondition.SnowFreezingRain] = new(Icons.SnowFreezingRain, Themes.Cold),
            [WeatherCondition.Sleet] = new(Icons.Sleet, Themes.Cold),
            [WeatherCondition.Rain] = new(Icons.Rain, Themes.Rain),
            [WeatherCondition.RainShowers] = new(Icons.ShowersDay, Themes.Rain),
            [WeatherCondition.Thunderstorm] = new(Icons.Thunderstorm, Themes.Storm),
            [WeatherCondition.Tornado] = new(Icons.Tornado, Themes.Storm),
            [WeatherCondition.Hurricane] = new(Icons.Hurricane, Themes.Storm),
            [WeatherCondition.TropicalStorm] = new(Icons.TropicalStorm, Themes.Storm),
            [WeatherCondition.Dust] = new(Icons.Dust, Themes.Dust),
            [WeatherCondition.Smoke] = new(Icons.Smoke, Themes.Fog),
            [WeatherCondition.Haze] = new(Icons.Haze, Themes.Fog),
            [WeatherCondition.Hot] = new(Icons.Hot, Themes.Heat),
            [WeatherCondition.Cold] = new(Icons.Cold, Themes.Cold),
            [WeatherCondition.Blizzard] = new(Icons.Blizzard, Themes.Snow),
            [WeatherCondition.Fog] = new(Icons.Fog, Themes.Fog),
        };

        /// <summary>
        /// Выбирает готовое оформление; неизвестное явление получает нейтральные иконку и тему в любое время суток.
        /// </summary>
        internal static Appearance Map(WeatherCondition condition, bool isDaytime)
        {
            if (_dayAppearances.TryGetValue(condition, out var appearance) is false)
                return _unknown;

            if (isDaytime)
                return appearance;

            var iconCode = condition switch
            {
                WeatherCondition.Clear => Icons.ClearNight,
                WeatherCondition.PartlyCloudy => Icons.PartlyCloudyNight,
                WeatherCondition.MostlyCloudy => Icons.MostlyCloudyNight,
                WeatherCondition.RainShowers => Icons.ShowersNight,
                _ => appearance.IconCode
            };
            var themeCode = condition switch
            {
                WeatherCondition.Thunderstorm
                    or WeatherCondition.Tornado
                    or WeatherCondition.Hurricane
                    or WeatherCondition.TropicalStorm
                    or WeatherCondition.Blizzard => Themes.Storm,
                _ => Themes.Night
            };

            return new(iconCode, themeCode);
        }

        /// <summary>
        /// Согласованная пара ключей локальных каталогов для одного показа погоды.
        /// </summary>
        internal readonly struct Appearance
        {
            internal string IconCode { get; }
            internal string ThemeCode { get; }

            /// <summary>
            /// Сохраняет согласованную пару ключей иконки и темы для показа погоды.
            /// </summary>
            internal Appearance(string iconCode, string themeCode)
            {
                IconCode = iconCode;
                ThemeCode = themeCode;
            }
        }
    }
}