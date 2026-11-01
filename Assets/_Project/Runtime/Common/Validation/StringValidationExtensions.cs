namespace Project.Common.Validation
{
    /// <summary>
    /// Предоставляет булеву проверку пустых и пробельных строк, включая null.
    /// </summary>
    public static class StringValidationExtensions
    {
        /// <summary>
        /// Возвращает true для null, пустой строки и строки только из пробельных символов.
        /// </summary>
        public static bool IsBlank(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}