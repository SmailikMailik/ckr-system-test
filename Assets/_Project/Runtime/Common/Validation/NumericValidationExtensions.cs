namespace Project.Common.Validation
{
    /// <summary>
    /// Предоставляет булевы предикаты числовых ограничений с учётом представления числа.
    /// </summary>
    public static class NumericValidationExtensions
    {
        /// <summary>
        /// Проверяет включительный диапазон int; перевёрнутые границы дают false для любого значения.
        /// </summary>
        public static bool IsInRange(this int value, int minimum, int maximum)
        {
            return value >= minimum && value <= maximum;
        }

        /// <summary>
        /// Проверяет строгую положительность целого числа int.
        /// </summary>
        public static bool IsPositive(this int value)
        {
            return ((long)value).IsPositive();
        }

        /// <summary>
        /// Проверяет неотрицательность целого числа int.
        /// </summary>
        public static bool IsNonNegative(this int value)
        {
            return ((long)value).IsNonNegative();
        }

        /// <summary>
        /// Проверяет строгую положительность целого числа long.
        /// </summary>
        public static bool IsPositive(this long value)
        {
            return value > 0;
        }

        /// <summary>
        /// Проверяет неотрицательность целого числа long.
        /// </summary>
        public static bool IsNonNegative(this long value)
        {
            return value >= 0;
        }

        /// <summary>
        /// Проверяет строгую положительность числа decimal.
        /// </summary>
        public static bool IsPositive(this decimal value)
        {
            return value > 0;
        }

        /// <summary>
        /// Проверяет неотрицательность числа decimal.
        /// </summary>
        public static bool IsNonNegative(this decimal value)
        {
            return value >= 0;
        }

        /// <summary>
        /// Проверяет строгую положительность и конечность числа float; NaN отклоняется.
        /// </summary>
        public static bool IsPositiveFinite(this float value)
        {
            return ((double)value).IsPositiveFinite();
        }

        /// <summary>
        /// Проверяет неотрицательность и конечность числа float; NaN отклоняется.
        /// </summary>
        public static bool IsNonNegativeFinite(this float value)
        {
            return ((double)value).IsNonNegativeFinite();
        }

        /// <summary>
        /// Проверяет строгую положительность и конечность числа double; NaN отклоняется.
        /// </summary>
        public static bool IsPositiveFinite(this double value)
        {
            return double.IsFinite(value) && value > 0;
        }

        /// <summary>
        /// Проверяет неотрицательность и конечность числа double; NaN отклоняется.
        /// </summary>
        public static bool IsNonNegativeFinite(this double value)
        {
            return double.IsFinite(value) && value >= 0;
        }
    }
}