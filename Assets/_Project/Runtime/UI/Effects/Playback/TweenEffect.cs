using DG.Tweening;
using UnityEngine;

namespace Project.UI.Effects.Playback
{
    /// <summary>
    /// Владеет одним корневым tween, заменяет его при повторном запуске и сбрасывает состояние при остановке.
    /// </summary>
    internal abstract class TweenEffect : EffectBehaviour
    {
        [SerializeField, Tooltip("Продолжать UI-анимацию при Time.timeScale = 0.")] private bool _independentUpdate = true;

        private Tween _tween;

        /// <summary>
        /// Заменяет прежнюю анимацию и явно запускает новый tween независимо от глобальных defaults DOTween.
        /// </summary>
        protected sealed override void OnPlay()
        {
            OnStop();
            _tween = CreateTween();

            if (_tween is null)
                return;

            _tween.SetAutoKill(true)
                .SetRecyclable(false)
                .SetUpdate(_independentUpdate)
                .SetLink(gameObject)
                .OnKill(() => _tween = null);
            _tween.Play();
        }

        /// <summary>
        /// Отменяет текущий tween без завершения и восстанавливает состояние целевого объекта.
        /// </summary>
        protected sealed override void OnStop()
        {
            _tween?.Kill();
            _tween = null;
            RestoreState();
        }

        /// <summary>
        /// Создаёт новый корневой tween и начальное состояние; null означает настроенное отсутствие анимации.
        /// </summary>
        protected abstract Tween CreateTween();

        /// <summary>
        /// Возвращает исходное состояние, захваченное конкретным эффектом до первого запуска.
        /// </summary>
        protected abstract void RestoreState();
    }
}