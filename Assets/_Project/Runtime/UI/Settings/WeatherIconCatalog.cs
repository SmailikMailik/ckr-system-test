using System;
using UnityEngine;

namespace Project.UI.Settings
{
    /// <summary>
    /// Сопоставляет сериализованные коды погоды с локальными изображениями.
    /// </summary>
    internal sealed class WeatherIconCatalog : ScriptableObject
    {
        [SerializeField] private Sprite _fallback;
        [SerializeField] private Entry[] _entries = Array.Empty<Entry>();

        /// <summary>
        /// Возвращает иконку по коду; для пустого или неизвестного кода и неназначенного Sprite выбирает резерв.
        /// </summary>
        /// <returns>Выбранный Sprite или null, если резервное изображение также не назначено.</returns>
        internal Sprite Get(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return _fallback;

            foreach (var entry in _entries)
            {
                if (entry.Code == code)
                    return entry.Sprite != null ? entry.Sprite : _fallback;
            }

            return _fallback;
        }

        /// <summary>
        /// Хранит соответствие кода погоды и локального изображения.
        /// </summary>
        /// <remarks>
        /// Изображение может быть не назначено; каталог использует резерв для такой записи.
        /// </remarks>
        [Serializable]
        private sealed class Entry
        {
            [SerializeField] private string _code;
            [SerializeField] private Sprite _sprite;

            internal string Code => _code;
            internal Sprite Sprite => _sprite;
        }
    }
}