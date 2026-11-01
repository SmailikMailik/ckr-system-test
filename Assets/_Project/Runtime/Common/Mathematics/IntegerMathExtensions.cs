using System;

namespace Project.Common.Mathematics
{
    /// <summary>
    /// Проверяет представимость целочисленных сумм и ограничивает их заданным диапазоном.
    /// </summary>
    public static class IntegerMathExtensions
    {
        /// <summary>
        /// Проверяет, представима ли точная сумма двух int в исходном типе.
        /// </summary>
        public static bool CanAdd(this int value, int delta)
        {
            var sum = (long)value + delta;
            return sum >= int.MinValue && sum <= int.MaxValue;
        }

        /// <summary>
        /// Проверяет, представима ли точная сумма двух long в исходном типе.
        /// </summary>
        public static bool CanAdd(this long value, long delta)
        {
            var sum = (decimal)value + delta;
            return sum >= long.MinValue && sum <= long.MaxValue;
        }

        /// <summary>
        /// Складывает int в long и ограничивает сумму включительным диапазоном.
        /// </summary>
        /// <exception cref="ArgumentException">Минимум больше максимума.</exception>
        public static int AddClamped(this int value, int delta, int minimum, int maximum)
        {
            return (int)Math.Clamp((long)value + delta, minimum, maximum);
        }

        /// <summary>
        /// Складывает long в decimal и ограничивает точную сумму включительным диапазоном.
        /// </summary>
        /// <exception cref="ArgumentException">Минимум больше максимума.</exception>
        public static long AddClamped(this long value, long delta, long minimum, long maximum)
        {
            return (long)Math.Clamp((decimal)value + delta, minimum, maximum);
        }
    }
}