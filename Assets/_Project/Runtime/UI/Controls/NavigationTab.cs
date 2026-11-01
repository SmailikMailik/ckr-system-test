using DG.Tweening;
using Project.UI.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Controls
{
    /// <summary>
    /// Связывает кнопку с панелью и плавно выделяет выбранную вкладку внутри общей навигации.
    /// </summary>
    [DisallowMultipleComponent]
    internal sealed class NavigationTab : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private UiButton _button;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private RectTransform _surface;
        [SerializeField] private Color _surfaceColor;
        [SerializeField] private Color _selectedSurfaceColor;
        [SerializeField] private Color _activeColor;
        [SerializeField, Min(0.01f)] private float _transitionDurationSeconds = 0.2f;
        [SerializeField, Min(0)] private float _selectedLift = 12;
        [SerializeField, Min(1)] private float _selectedIconScale = 1.08f;

        private const float HighlightSurfaceBlend = 0.06f;
        private const float PressedAccentBlend = 0.12f;
        private const float InactiveIconTint = 0.25f;

        private Tween _transition;
        private float _selection;
        private bool _hasSelection;
        private bool _selected;
        private Color _inactiveColor;

        internal UiButton Button => _button;
        internal string Title => _label.text;

        private void OnDisable()
        {
            StopTransition();

            if (_hasSelection)
                ApplySelection(_selected ? 1 : 0);
        }

        private void OnDestroy()
        {
            StopTransition();
        }

        /// <summary>
        /// Сразу переключает панель и анимирует её выделение, сохраняя область нажатия; первый показ
        /// оформляется мгновенно.
        /// </summary>
        internal void Show(bool selected, UiToolkitTheme theme)
        {
            _panel.SetActive(selected);

            if (_hasSelection && _selected == selected)
                return;

            var animate = _hasSelection && Application.isPlaying && isActiveAndEnabled;
            _selected = selected;
            _inactiveColor = theme.Muted;
            var colors = _button.Colors;
            colors.normalColor = selected ? _selectedSurfaceColor : _surfaceColor;
            colors.highlightedColor = Color.Lerp(colors.normalColor, _selectedSurfaceColor, HighlightSurfaceBlend);
            colors.pressedColor = Color.Lerp(_selectedSurfaceColor, _activeColor, PressedAccentBlend);
            colors.selectedColor = colors.normalColor;
            colors.fadeDuration = animate ? _transitionDurationSeconds : 0;
            _button.Colors = colors;
            StopTransition();
            _hasSelection = true;
            var target = selected ? 1f : 0f;

            if (animate is false)
            {
                ApplySelection(target);
                return;
            }

            _transition = DOTween.To(() => _selection, ApplySelection, target, _transitionDurationSeconds)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true)
                .SetAutoKill(true)
                .SetRecyclable(false)
                .SetTarget(this)
                .SetLink(gameObject)
                .OnKill(() => _transition = null);
            _transition.Play();
        }

        private void ApplySelection(float selection)
        {
            _selection = selection;
            _icon.color = Color.Lerp(Color.Lerp(Color.white, _inactiveColor, InactiveIconTint), Color.white, selection);
            _icon.rectTransform.localScale = Vector3.one * Mathf.Lerp(1, _selectedIconScale, selection);
            _label.color = Color.Lerp(_inactiveColor, _activeColor, selection);
            _surface.offsetMax = new(0, _selectedLift * selection);
        }

        private void StopTransition()
        {
            _transition?.Kill();
            _transition = null;
        }
    }
}