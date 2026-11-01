using System;
using UnityEngine;

namespace Project.UI.Settings
{
    /// <summary>
    /// Выбирает локальный фон и декор по ключу темы; резервное оформление назначается в каталоге.
    /// </summary>
    internal sealed class WeatherThemeCatalog : ScriptableObject
    {
        [SerializeField] private Entry _fallback;
        [SerializeField] private Entry[] _entries = Array.Empty<Entry>();

        /// <summary>
        /// Возвращает фон выбранной темы; неизвестный код и пустое изображение используют назначенный резерв.
        /// </summary>
        /// <returns>Изображение выбранной темы или резерва; <c>null</c>, когда изображение не назначено.</returns>
        internal Sprite Get(string code)
        {
            return FindEntry(code)?.Background;
        }

        /// <summary>
        /// Возвращает оформление темы целиком; резерв сохраняет согласованные фон, оттенок облаков и слои декора.
        /// </summary>
        /// <returns>Оформление темы или резерва; результат может быть <c>null</c> при неназначенном резерве.</returns>
        internal Entry GetStyle(string code)
        {
            return FindEntry(code);
        }

        private Entry FindEntry(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return _fallback;

            foreach (var entry in _entries)
            {
                if (entry.Code == code)
                    return entry.Background != null ? entry : _fallback;
            }

            return _fallback;
        }

        /// <summary>
        /// Хранит семантический код, сгенерированный фон и оформление неба.
        /// </summary>
        /// <remarks>
        /// Фон может быть не назначен; каталог использует резерв для такой записи.
        /// Alpha цвета <see cref="Entry.CloudTint"/> задаёт прозрачность облаков.
        /// <see cref="Entry.Stars"/> управляет звёздами и световыми бликами.
        /// <see cref="Entry.Rain"/> и <see cref="Entry.Snow"/> включают слои осадков.
        /// </remarks>
        [Serializable]
        internal sealed class Entry
        {
            [SerializeField] private string _code;
            [SerializeField] private Sprite _background;
            [SerializeField] private Color _cloudTint = Color.white;
            [SerializeField] private bool _stars;
            [SerializeField] private bool _rain;
            [SerializeField] private bool _snow;

            internal string Code => _code;
            internal Sprite Background => _background;
            internal Color CloudTint => _cloudTint;
            internal bool Stars => _stars;
            internal bool Rain => _rain;
            internal bool Snow => _snow;
        }
    }
}