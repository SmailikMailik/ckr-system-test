using UnityEngine;

namespace Project.UI.Layout
{
    /// <summary>
    /// Ограничивает содержимое полноэкранного Canvas безопасной областью экрана.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    internal sealed class UiSafeArea : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Rect _lastArea;
        private Vector2Int _lastSize;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            ApplySafeArea();
        }

        private void Update()
        {
            if (_lastArea != Screen.safeArea || _lastSize.x != Screen.width || _lastSize.y != Screen.height)
                ApplySafeArea();
        }

        private void ApplySafeArea()
        {
            if (Screen.width <= 0 || Screen.height <= 0)
                return;

            _lastArea = Screen.safeArea;
            _lastSize = new(Screen.width, Screen.height);
            var minimum = _lastArea.min;
            var maximum = _lastArea.max;
            _rectTransform.anchorMin = new(minimum.x / Screen.width, minimum.y / Screen.height);
            _rectTransform.anchorMax = new(maximum.x / Screen.width, maximum.y / Screen.height);
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
        }
    }
}