using DG.Tweening;
using Project.UI.Effects.Playback;
using UnityEngine;

namespace Project.UI.Effects.Animations
{
    /// <summary>
    /// Анимирует только прозрачность CanvasGroup, оставляя маршрутизацию ввода владельцу интерфейса.
    /// </summary>
    [AddComponentMenu("Project/UI/Effects/Fade Effect")]
    internal sealed class FadeEffect : TweenEffect
    {
        [SerializeField] private CanvasGroup _target;
        [SerializeField, Range(0, 1)] private float _fromAlpha;
        [SerializeField, Range(0, 1)] private float _toAlpha = 1;
        [SerializeField, Min(0.01f)] private float _durationSeconds = 0.18f;
        [SerializeField] private Ease _ease = Ease.OutQuad;

        private bool _hasInitialAlpha;
        private float _initialAlpha;

        /// <summary>
        /// Сохраняет исходную прозрачность и создаёт переход между настроенными значениями alpha.
        /// </summary>
        protected override Tween CreateTween()
        {
            if (_hasInitialAlpha is false)
            {
                _initialAlpha = _target.alpha;
                _hasInitialAlpha = true;
            }

            _target.alpha = _fromAlpha;
            return _target.DOFade(_toAlpha, _durationSeconds).SetEase(_ease);
        }

        /// <summary>
        /// Возвращает исходную прозрачность, сохраняя значения interactable и blocksRaycasts.
        /// </summary>
        protected override void RestoreState()
        {
            if (_hasInitialAlpha && _target != null)
                _target.alpha = _initialAlpha;
        }
    }
}