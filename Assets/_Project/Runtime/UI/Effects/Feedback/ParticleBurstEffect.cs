using Project.UI.Effects.Playback;
using UnityEngine;

namespace Project.UI.Effects.Feedback
{
    /// <summary>
    /// Выпускает настроенное количество частиц и очищает принадлежащую эффекту систему при остановке.
    /// </summary>
    [AddComponentMenu("Project/UI/Effects/Particle Burst Effect")]
    internal sealed class ParticleBurstEffect : EffectBehaviour
    {
        [SerializeField] private ParticleSystem _particles;
        [SerializeField, Min(0)] private int _count = 12;

        /// <summary>
        /// Добавляет новый burst, сохраняя живые частицы предыдущих сигналов.
        /// </summary>
        protected override void OnPlay()
        {
            _particles.Emit(_count);
        }

        /// <summary>
        /// Останавливает систему вместе с дочерними системами и очищает оставшиеся частицы.
        /// </summary>
        protected override void OnStop()
        {
            if (_particles != null)
                _particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}