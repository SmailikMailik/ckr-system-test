using Project.Common.Mathematics;
using UnityEngine;

namespace Project.UI.Controls
{
    /// <summary>
    /// Отображает долю значения горизонтальным заполнением слева направо.
    /// </summary>
    internal sealed class UiBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _fill;

        /// <summary>
        /// Обновляет заполнение в диапазоне [0, 1]; максимум должен быть положительным.
        /// </summary>
        internal void SetValue(int current, int maximum)
        {
            _fill.anchorMax = new(RatioMath.NormalizeClamped(current, maximum), 1);
        }
    }
}