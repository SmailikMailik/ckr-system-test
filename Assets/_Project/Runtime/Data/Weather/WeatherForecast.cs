namespace Project.Data.Weather
{
    /// <summary>
    /// Содержит температуру, описание, погодное явление и время суток одного периода прогноза.
    /// </summary>
    public sealed class WeatherForecast
    {
        /// <summary>
        /// Числовое значение температуры; Unit указывает единицу, например C для градусов Цельсия.
        /// </summary>
        public double Temperature { get; }

        /// <summary>
        /// Обозначение единицы температуры, например C (Цельсий) или F (Фаренгейт).
        /// </summary>
        public string Unit { get; }

        /// <summary>
        /// Краткое описание погоды для выбранного периода.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Погодное явление; Unknown обозначает отсутствие распознанного состояния в ответе источника.
        /// </summary>
        public WeatherCondition Condition { get; }

        /// <summary>
        /// Время суток периода, определённое источником прогноза.
        /// </summary>
        public bool IsDaytime { get; }

        /// <summary>
        /// Сохраняет температуру, описание, погодное явление и время суток периода.
        /// </summary>
        public WeatherForecast(
            double temperature,
            string unit,
            string description,
            WeatherCondition condition,
            bool isDaytime = true)
        {
            Temperature = temperature;
            Unit = unit;
            Description = description;
            Condition = condition;
            IsDaytime = isDaytime;
        }
    }
}