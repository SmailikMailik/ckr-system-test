using System;

namespace Project.Common.Validation
{
    /// <summary>
    /// Преобразует результаты общих проверок в стандартные исключения с исходным именем аргумента.
    /// </summary>
    public static class ArgumentGuard
    {
        /// <summary>
        /// Отклоняет int вне включительного диапазона; перевёрнутые границы отклоняют любое значение.
        /// </summary>
        public static void ThrowIfOutOfRange(int value, int minimum, int maximum, string parameterName)
        {
            ThrowIfInvalidNumber(value.IsInRange(minimum, maximum), parameterName);
        }

        /// <summary>
        /// Отклоняет неположительное значение int, сохраняя имя аргумента.
        /// </summary>
        public static void ThrowIfNotPositive(int value, string parameterName)
        {
            ThrowIfInvalidNumber(value.IsPositive(), parameterName);
        }

        /// <summary>
        /// Отклоняет отрицательное значение int, сохраняя имя аргумента.
        /// </summary>
        public static void ThrowIfNegative(int value, string parameterName)
        {
            ThrowIfInvalidNumber(value.IsNonNegative(), parameterName);
        }

        /// <summary>
        /// Отклоняет неположительное значение long, сохраняя имя аргумента.
        /// </summary>
        public static void ThrowIfNotPositive(long value, string parameterName)
        {
            ThrowIfInvalidNumber(value.IsPositive(), parameterName);
        }

        /// <summary>
        /// Отклоняет отрицательное значение long, сохраняя имя аргумента.
        /// </summary>
        public static void ThrowIfNegative(long value, string parameterName)
        {
            ThrowIfInvalidNumber(value.IsNonNegative(), parameterName);
        }

        /// <summary>
        /// Отклоняет неположительное значение decimal, сохраняя имя аргумента.
        /// </summary>
        public static void ThrowIfNotPositive(decimal value, string parameterName)
        {
            ThrowIfInvalidNumber(value.IsPositive(), parameterName);
        }

        /// <summary>
        /// Отклоняет отрицательное значение decimal, сохраняя имя аргумента.
        /// </summary>
        public static void ThrowIfNegative(decimal value, string parameterName)
        {
            ThrowIfInvalidNumber(value.IsNonNegative(), parameterName);
        }

        /// <summary>
        /// Отклоняет неположительное или неконечное значение float, включая NaN.
        /// </summary>
        public static void ThrowIfNotPositiveFinite(float value, string parameterName)
        {
            ThrowIfInvalidNumber(value.IsPositiveFinite(), parameterName);
        }

        /// <summary>
        /// Отклоняет отрицательное или неконечное значение float, включая NaN.
        /// </summary>
        public static void ThrowIfNegativeOrNotFinite(float value, string parameterName)
        {
            ThrowIfInvalidNumber(value.IsNonNegativeFinite(), parameterName);
        }

        /// <summary>
        /// Отклоняет неположительное или неконечное значение double, включая NaN.
        /// </summary>
        public static void ThrowIfNotPositiveFinite(double value, string parameterName)
        {
            ThrowIfInvalidNumber(value.IsPositiveFinite(), parameterName);
        }

        /// <summary>
        /// Отклоняет отрицательное или неконечное значение double, включая NaN.
        /// </summary>
        public static void ThrowIfNegativeOrNotFinite(double value, string parameterName)
        {
            ThrowIfInvalidNumber(value.IsNonNegativeFinite(), parameterName);
        }

        /// <summary>
        /// Отклоняет NaN и обе бесконечности, сохраняя имя аргумента; конечные числа любого знака допустимы.
        /// </summary>
        public static void ThrowIfNotFinite(double value, string parameterName)
        {
            ThrowIfInvalidNumber(double.IsFinite(value), parameterName);
        }

        /// <summary>
        /// Отклоняет только CLR-null, не используя перегруженные операторы сравнения объектов.
        /// </summary>
        public static void ThrowIfNull(object value, string parameterName)
        {
            if (value is null)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Отклоняет null, пустую строку и строку только из пробельных символов.
        /// </summary>
        public static void ThrowIfBlank(string value, string parameterName)
        {
            if (value.IsBlank())
                throw new ArgumentException("Значение не должно быть пустым или пробельным.", parameterName);
        }

        /// <summary>
        /// Применяет общую политику ошибки к результату числового предиката.
        /// </summary>
        private static void ThrowIfInvalidNumber(bool isValid, string parameterName)
        {
            if (isValid is false)
                throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}