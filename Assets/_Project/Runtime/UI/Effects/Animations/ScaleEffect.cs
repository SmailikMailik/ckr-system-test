using DG.Tweening;
using Project.UI.Effects.Playback;
using UnityEngine;

namespace Project.UI.Effects.Animations
{
    /// <summary>
    /// Анимирует локальный масштаб относительно исходного размера объекта и восстанавливает его при остановке.
    /// </summary>
    [AddComponentMenu("Project/UI/Effects/Scale Effect")]
    internal sealed class ScaleEffect : TweenEffect
    {
        [SerializeField, Tooltip("Объект, масштаб которого принадлежит этому эффекту.")] private Transform _target;
        [SerializeField, Tooltip("Начальный множитель исходного localScale по каждой оси.")] private Vector3 _fromScale = Vector3.one;
        [SerializeField, Tooltip("Конечный множитель исходного localScale по каждой оси.")] private Vector3 _toScale = Vector3.one;
        [SerializeField, Min(0.01f)] private float _durationSeconds = 0.18f;
        [SerializeField] private Ease _ease = Ease.OutQuad;
        [SerializeField, Tooltip("Число повторений DOTween; -1 означает бесконечное повторение.")] private int _loops = 1;
        [SerializeField] private LoopType _loopType = LoopType.Restart;

        private bool _hasInitialScale;
        private Vector3 _initialScale;

        /// <summary>
        /// Сохраняет исходный масштаб при первом запуске и создаёт анимацию между настроенными множителями.
        /// </summary>
        protected override Tween CreateTween()
        {
            if (_hasInitialScale is false)
            {
                _initialScale = _target.localScale;
                _hasInitialScale = true;
            }

            _target.localScale = Vector3.Scale(_initialScale, _fromScale);
            return _target.DOScale(Vector3.Scale(_initialScale, _toScale), _durationSeconds)
                .SetEase(_ease)
                .SetLoops(_loops, _loopType);
        }

        /// <summary>
        /// Возвращает первоначальный масштаб, если целевой объект ещё существует.
        /// </summary>
        protected override void RestoreState()
        {
            if (_hasInitialScale && _target != null)
                _target.localScale = _initialScale;
        }
    }
}