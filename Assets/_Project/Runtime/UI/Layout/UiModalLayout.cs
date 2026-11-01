using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Layout
{
    /// <summary>
    /// Подстраивает высоту описания всплывающего окна под текст и ограничивает её доступной областью родителя.
    /// </summary>
    internal sealed class UiModalLayout : MonoBehaviour
    {
        [SerializeField] private TMP_Text _messageLabel;
        [SerializeField] private LayoutElement _contentLayout;
        [SerializeField] private float _maximumBodyHeight = 600;
        [SerializeField] private float _chromeHeight = 468;
        [SerializeField, Range(0.1f, 1)] private float _maximumScreenFraction = 0.85f;

        private const float MinimumBodyHeight = 96;

        private string _lastText;
        private Vector2 _lastSize;
        private float _lastParentHeight;
        private int _lastContentPadding;

        private void LateUpdate()
        {
            var parent = transform.parent as RectTransform;
            var parentHeight = parent != null ? parent.rect.height : _chromeHeight + _maximumBodyHeight;
            var size = _messageLabel.rectTransform.rect.size;
            var content = _messageLabel.transform.parent;
            var group = content != null ? content.GetComponent<LayoutGroup>() : null;
            var contentPadding = group != null ? group.padding.vertical : 0;

            if (_lastText == _messageLabel.text && _lastSize == size && _lastParentHeight == parentHeight
                && _lastContentPadding == contentPadding)
                return;

            _lastText = _messageLabel.text;
            _lastSize = size;
            _lastParentHeight = parentHeight;
            _lastContentPadding = contentPadding;
            var availableHeight = Mathf.Clamp(
                parentHeight * _maximumScreenFraction - _chromeHeight,
                MinimumBodyHeight,
                Mathf.Max(MinimumBodyHeight, _maximumBodyHeight));
            var preferredHeight = _messageLabel.GetPreferredValues(_messageLabel.text, size.x, Mathf.Infinity).y
                + contentPadding;
            _contentLayout.preferredHeight = Mathf.Clamp(preferredHeight, MinimumBodyHeight, availableHeight);
        }
    }
}