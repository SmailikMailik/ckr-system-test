using System;
using Project.Common.Validation;

namespace Project.Common.Mathematics
{
    /// <summary>
    /// Вычисляет доли значений и ограничивает результат диапазоном [0, 1].
    /// </summary>
    public static class RatioMath
    {
        /// <summary>
        /// Возвращает долю int в виде float в диапазоне [0, 1]; максимум должен быть положительным.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Максимум не является положительным.</exception>
        public static float NormalizeClamped(int value, int maximum)
        {
            return (float)NormalizeClamped((double)value, maximum);
        }

        /// <summary>
        /// Возвращает долю long в диапазоне [0, 1]; максимум положителен, точность ограничена представлением
        /// double.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Максимум не является положительным.</exception>
        public static double NormalizeClamped(long value, long maximum)
        {
            return NormalizeClamped((double)value, maximum);
        }

        /// <summary>
        /// Возвращает долю конечного float от положительного конечного максимума в диапазоне [0, 1].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Аргумент не удовлетворяет числовому контракту.</exception>
        public static float NormalizeClamped(float value, float maximum)
        {
            return (float)NormalizeClamped((double)value, maximum);
        }

        /// <summary>
        /// Возвращает долю конечного double от положительного конечного максимума в диапазоне [0, 1].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Аргумент не удовлетворяет числовому контракту.</exception>
        public static double NormalizeClamped(double value, double maximum)
        {
            ArgumentGuard.ThrowIfNotFinite(value, nameof(value));

            ArgumentGuard.ThrowIfNotPositiveFinite(maximum, nameof(maximum));

            return Math.Clamp(value, 0, maximum) / maximum;
        }

        /// <summary>
        /// Возвращает долю decimal от положительного максимума в диапазоне [0, 1] без переполнения деления.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Максимум не является положительным.</exception>
        public static decimal NormalizeClamped(decimal value, decimal maximum)
        {
            ArgumentGuard.ThrowIfNotPositive(maximum, nameof(maximum));

            return Math.Clamp(value, 0, maximum) / maximum;
        }
    }
}