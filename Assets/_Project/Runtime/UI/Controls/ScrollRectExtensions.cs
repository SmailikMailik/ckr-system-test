using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Controls
{
    /// <summary>
    /// Общие операции прокрутки интерфейса.
    /// </summary>
    internal static class ScrollRectExtensions
    {
        /// <summary>
        /// Останавливает инерцию и возвращает содержимое к началу вертикальной прокрутки.
        /// </summary>
        internal static void ResetToTop(this ScrollRect scrollRect)
        {
            scrollRect.StopMovement();
            scrollRect.content.anchoredPosition = Vector2.zero;
            scrollRect.verticalNormalizedPosition = 1;
        }
    }
}