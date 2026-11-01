using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Data.Remote.Serialization;
using Project.Data.Weather;

namespace Project.Data.Remote.Nws
{
    /// <summary>
    /// Проверяет типы и структуру GeoJSON Feature NWS и извлекает данные первого периода прогноза.
    /// </summary>
    internal static class NwsForecastParser
    {
        private const string ForecastFeatureType = "Feature";
        private const string CelsiusUnit = "C";
        private const string FahrenheitUnit = "F";
        private const string CelsiusUnitCode = "wmoUnit:degC";
        private const string FahrenheitUnitCode = "wmoUnit:degF";
        private const string DaySegment = "day";
        private const string NightSegment = "night";
        private const string NightIconPath = "/night/";
        private const string MeasurementUnitPropertyName = "unitCode";
        private const string MeasurementValuePropertyName = "value";

        private static readonly Dictionary<string, WeatherCondition> _conditions = new(StringComparer.Ordinal)
        {
            ["skc"] = WeatherCondition.Clear,
            ["few"] = WeatherCondition.PartlyCloudy,
            ["sct"] = WeatherCondition.PartlyCloudy,
            ["bkn"] = WeatherCondition.MostlyCloudy,
            ["ovc"] = WeatherCondition.Overcast,
            ["wind_skc"] = WeatherCondition.Windy,
            ["wind_few"] = WeatherCondition.Windy,
            ["wind_sct"] = WeatherCondition.Windy,
            ["wind_bkn"] = WeatherCondition.Windy,
            ["wind_ovc"] = WeatherCondition.Windy,
            ["snow"] = WeatherCondition.Snow,
            ["rain_snow"] = WeatherCondition.RainSnow,
            ["rain_sleet"] = WeatherCondition.RainSleet,
            ["snow_sleet"] = WeatherCondition.SnowSleet,
            ["fzra"] = WeatherCondition.FreezingRain,
            ["rain_fzra"] = WeatherCondition.FreezingRain,
            ["snow_fzra"] = WeatherCondition.SnowFreezingRain,
            ["sleet"] = WeatherCondition.Sleet,
            ["rain"] = WeatherCondition.Rain,
            ["rain_showers"] = WeatherCondition.RainShowers,
            ["rain_showers_hi"] = WeatherCondition.RainShowers,
            ["tsra"] = WeatherCondition.Thunderstorm,
            ["tsra_sct"] = WeatherCondition.Thunderstorm,
            ["tsra_hi"] = WeatherCondition.Thunderstorm,
            ["tornado"] = WeatherCondition.Tornado,
            ["hurricane"] = WeatherCondition.Hurricane,
            ["tropical_storm"] = WeatherCondition.TropicalStorm,
            ["dust"] = WeatherCondition.Dust,
            ["smoke"] = WeatherCondition.Smoke,
            ["haze"] = WeatherCondition.Haze,
            ["hot"] = WeatherCondition.Hot,
            ["cold"] = WeatherCondition.Cold,
            ["blizzard"] = WeatherCondition.Blizzard,
            ["fog"] = WeatherCondition.Fog,
        };

        private static readonly JsonSerializerSettings _settings = new()
        {
            DateParseHandling = DateParseHandling.None,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            CheckAdditionalContent = true
        };

        /// <summary>
        /// Возвращает первый период Feature; неизвестные поля допустимы, неполные и неверные данные отклоняются.
        /// </summary>
        /// <exception cref="FormatException">Ответ не соответствует поддерживаемому контракту прогноза NWS.</exception>
        internal static WeatherForecast Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new FormatException("Получен пустой ответ NWS.");

            try
            {
                var envelope = JsonConvert.DeserializeObject<ForecastEnvelope>(json, _settings);

                if (envelope?.Type != ForecastFeatureType || envelope.Properties?.Periods is null
                    || envelope.Properties.Periods.Length == 0)
                    throw new FormatException("Ответ NWS не содержит Feature с периодами прогноза.");

                var period = envelope.Properties.Periods[0];

                if (period is null || string.IsNullOrWhiteSpace(period.ShortForecast))
                    throw new FormatException("Первый период NWS не содержит описания погоды.");

                var temperature = ReadTemperature(period);
                var daytime = ReadDaytime(period);
                var condition = ReadCondition(period.Icon);
                return new(
                    temperature.Value,
                    temperature.Unit,
                    period.ShortForecast,
                    condition,
                    daytime);
            }
            catch (JsonException exception)
            {
                throw new FormatException("Не удалось разобрать ответ прогноза NWS.", exception);
            }
        }

        private static TemperatureReading ReadTemperature(ForecastPeriod period)
        {
            if (period.Temperature?.Type == JTokenType.Integer)
            {
                if (period.TemperatureUnit is not (CelsiusUnit or FahrenheitUnit))
                    throw new FormatException("В периоде NWS отсутствует поддерживаемая единица температуры.");

                return new(ReadNumber(period.Temperature), period.TemperatureUnit);
            }

            if (period.Temperature is not JObject measurement)
                throw new FormatException("В периоде NWS отсутствует поддерживаемое значение температуры.");

            var unitCode = measurement[MeasurementUnitPropertyName];

            if (unitCode?.Type != JTokenType.String)
                throw new FormatException("Измерение температуры NWS не содержит unitCode.");

            var unit = unitCode.Value<string>() switch
            {
                CelsiusUnitCode => CelsiusUnit,
                FahrenheitUnitCode => FahrenheitUnit,
                _ => throw new FormatException("Единица измерения температуры NWS не поддерживается.")
            };

            if (period.TemperatureUnit is not null && period.TemperatureUnit != unit)
                throw new FormatException("Единицы температуры в ответе NWS противоречат друг другу.");

            return new(ReadNumber(measurement[MeasurementValuePropertyName]), unit);
        }

        private static bool ReadDaytime(ForecastPeriod period)
        {
            if (period.IsDaytime is null)
                return period.Icon?.Contains(NightIconPath) is not true;

            if (period.IsDaytime.Type != JTokenType.Boolean)
                throw new FormatException("Время суток периода NWS должно быть логическим значением.");

            return period.IsDaytime.Value<bool>();
        }

        private static WeatherCondition ReadCondition(string iconUrl)
        {
            if (Uri.TryCreate(iconUrl, UriKind.Absolute, out var uri) is false)
                return WeatherCondition.Unknown;

            var segments = uri.AbsolutePath.Split('/');

            // Составной прогноз сохраняет порядок состояний; вероятность не является частью кода.
            for (var i = 0; i < segments.Length; i++)
            {
                if (segments[i] is not (DaySegment or NightSegment))
                    continue;

                for (var j = i + 1; j < segments.Length; j++)
                {
                    var code = segments[j].Split(',')[0];

                    if (_conditions.TryGetValue(code, out var condition))
                        return condition;
                }

                break;
            }

            return WeatherCondition.Unknown;
        }

        private static double ReadNumber(JToken token)
        {
            if (token?.Type is not (JTokenType.Integer or JTokenType.Float))
                throw new FormatException("Температура NWS должна содержать числовое значение.");

            var value = (double)token;

            if (double.IsFinite(value) is false)
                throw new FormatException("Температура NWS должна быть конечным числом.");

            return value;
        }

        /// <summary>
        /// Сохраняет значение температуры вместе с проверенной единицей одного периода прогноза.
        /// </summary>
        private readonly struct TemperatureReading
        {
            internal double Value { get; }
            internal string Unit { get; }

            /// <summary>
            /// Сохраняет прочитанное значение температуры и поддерживаемую единицу измерения.
            /// </summary>
            internal TemperatureReading(double value, string unit)
            {
                Value = value;
                Unit = unit;
            }
        }

        /// <summary>
        /// Описывает используемые поля корневого GeoJSON Feature.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        private sealed class ForecastEnvelope
        {
            [JsonProperty("type"), JsonConverter(typeof(StrictStringConverter))]
            internal string Type { get; private set; }

            [JsonProperty("properties")]
            internal ForecastProperties Properties { get; private set; }
        }

        /// <summary>
        /// Содержит периоды прогноза из свойств GeoJSON Feature NWS.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        private sealed class ForecastProperties
        {
            [JsonProperty("periods")]
            internal ForecastPeriod[] Periods { get; private set; }
        }

        /// <summary>
        /// Описывает нужные поля периода, включая оба документированных формата температуры NWS.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        private sealed class ForecastPeriod
        {
            [JsonProperty("temperature")]
            internal JToken Temperature { get; private set; }

            [JsonProperty("temperatureUnit"), JsonConverter(typeof(StrictStringConverter))]
            internal string TemperatureUnit { get; private set; }

            [JsonProperty("shortForecast"), JsonConverter(typeof(StrictStringConverter))]
            internal string ShortForecast { get; private set; }

            [JsonProperty("isDaytime")]
            internal JToken IsDaytime { get; private set; }

            [JsonProperty("icon"), JsonConverter(typeof(StrictStringConverter))]
            internal string Icon { get; private set; }
        }
    }
}