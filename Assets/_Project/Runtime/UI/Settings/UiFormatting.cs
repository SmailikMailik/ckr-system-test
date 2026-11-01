using System.Globalization;

namespace Project.UI.Settings
{
    /// <summary>
    /// Задаёт культуру отображения чисел и времени в UI по актуальным региональным настройкам устройства.
    /// </summary>
    internal static class UiFormatting
    {
        internal static CultureInfo Culture => CultureInfo.CurrentCulture;
    }
}