namespace Project.Data.Weather
{
    /// <summary>
    /// Перечисляет погодные явления, передаваемые источником прогноза.
    /// </summary>
    public enum WeatherCondition : byte
    {
        Unknown = 0,
        Clear = 1,
        PartlyCloudy = 2,
        MostlyCloudy = 3,
        Overcast = 4,
        Windy = 5,
        Snow = 6,
        RainSnow = 7,
        RainSleet = 8,
        SnowSleet = 9,
        FreezingRain = 10,
        SnowFreezingRain = 11,
        Sleet = 12,
        Rain = 13,
        RainShowers = 14,
        Thunderstorm = 15,
        Tornado = 16,
        Hurricane = 17,
        TropicalStorm = 18,
        Dust = 19,
        Smoke = 20,
        Haze = 21,
        Hot = 22,
        Cold = 23,
        Blizzard = 24,
        Fog = 25,
    }
}