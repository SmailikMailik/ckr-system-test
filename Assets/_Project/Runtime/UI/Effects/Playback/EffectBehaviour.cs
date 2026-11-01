using UnityEngine;

namespace Project.UI.Effects.Playback
{
    /// <summary>
    /// Принимает локальный сигнал эффекта и освобождает принадлежащие ему ресурсы при отключении.
    /// </summary>
    internal abstract class EffectBehaviour : MonoBehaviour
    {
        private bool _requiresStop;

        /// <summary>
        /// Запускает активный эффект; поведение повторного сигнала определяет конкретный обработчик.
        /// </summary>
        public void Play()
        {
            // Сигнал из OnEnable может прийти раньше Awake другого компонента.
            // Обработчики инициализируются при Play, поэтому проверяем настроенную активность напрямую.
            if (enabled is false || gameObject.activeInHierarchy is false)
                return;

            _requiresStop = true;
            OnPlay();
        }

        /// <summary>
        /// Останавливает эффект и восстанавливает исходное состояние не более одного раза на период воспроизведения.
        /// </summary>
        public void Stop()
        {
            if (_requiresStop is false)
                return;

            _requiresStop = false;
            OnStop();
        }

        private void OnDisable()
        {
            Stop();
        }

        /// <summary>
        /// Выполняет локальный отклик эффекта на сигнал воспроизведения.
        /// </summary>
        protected abstract void OnPlay();

        /// <summary>
        /// Освобождает собственные ресурсы и возвращает визуальное состояние эффекта.
        /// </summary>
        protected abstract void OnStop();
    }
}