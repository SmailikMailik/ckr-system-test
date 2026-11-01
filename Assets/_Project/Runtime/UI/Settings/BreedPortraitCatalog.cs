using System;
using UnityEngine;

namespace Project.UI.Settings
{
    /// <summary>
    /// Выбирает общий портрет строки и окна по стабильному ID породы.
    /// </summary>
    internal sealed class BreedPortraitCatalog : ScriptableObject
    {
        [SerializeField] private Sprite _fallback;
        [SerializeField] private Entry[] _entries = Array.Empty<Entry>();

        /// <summary>
        /// Возвращает портрет по ID; для пустого или неизвестного ID и неназначенного Sprite выбирает резерв.
        /// </summary>
        /// <returns>Выбранный Sprite или null, если резервное изображение также не назначено.</returns>
        internal Sprite Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                return _fallback;

            foreach (var entry in _entries)
            {
                if (entry.Id == id)
                    return entry.Sprite != null ? entry.Sprite : _fallback;
            }

            return _fallback;
        }

        /// <summary>
        /// Хранит соответствие стабильного API ID породы и локального изображения.
        /// </summary>
        /// <remarks>
        /// Изображение может быть не назначено; каталог использует резерв для такой записи.
        /// </remarks>
        [Serializable]
        private sealed class Entry
        {
            [SerializeField] private string _id;
            [SerializeField] private Sprite _sprite;

            internal string Id => _id;
            internal Sprite Sprite => _sprite;
        }
    }
}