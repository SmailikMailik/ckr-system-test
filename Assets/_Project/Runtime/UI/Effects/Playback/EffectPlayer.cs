using UnityEngine;

namespace Project.UI.Effects.Playback
{
    /// <summary>
    /// Передаёт один локальный сигнал настроенному в Inspector набору независимых эффектов.
    /// </summary>
    [AddComponentMenu("Project/UI/Effects/Effect Player")]
    [DisallowMultipleComponent]
    internal sealed class EffectPlayer : MonoBehaviour
    {
        [SerializeField, Tooltip("Обработчики локального сигнала; каждый эффект сам владеет своими ресурсами.")] private EffectBehaviour[] _effects = System.Array.Empty<EffectBehaviour>();

        private void OnDisable()
        {
            Stop();
        }

        /// <summary>
        /// Запускает настроенные эффекты, пока проигрыватель активен.
        /// </summary>
        public void Play()
        {
            // Сигнал из OnEnable может прийти раньше Awake другого компонента.
            // Обработчики инициализируются при Play, поэтому проверяем настроенную активность напрямую.
            if (enabled is false || gameObject.activeInHierarchy is false)
                return;

            foreach (var effect in _effects)
                effect.Play();
        }

        /// <summary>
        /// Останавливает только эффекты этого проигрывателя; повторная остановка безопасна.
        /// </summary>
        public void Stop()
        {
            foreach (var effect in _effects)
                effect.Stop();
        }
    }
}