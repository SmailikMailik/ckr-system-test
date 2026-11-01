using Project.UI.Effects.Playback;
using UnityEngine;

namespace Project.UI.Effects.Feedback
{
    /// <summary>
    /// Воспроизводит назначенный AudioClip или clip собственного AudioSource, допуская перекрытие коротких звуков.
    /// </summary>
    [AddComponentMenu("Project/UI/Effects/Audio Effect")]
    internal sealed class AudioEffect : EffectBehaviour
    {
        [SerializeField] private AudioSource _source;
        [SerializeField, Tooltip("Необязательный clip эффекта; при отсутствии используется clip AudioSource.")] private AudioClip _clip;

        /// <summary>
        /// Проигрывает короткий звук; отсутствие обоих clips означает настроенное отсутствие аудио.
        /// </summary>
        protected override void OnPlay()
        {
            var clip = _clip != null ? _clip : _source.clip;

            if (clip == null)
                return;

            _source.PlayOneShot(clip);
        }

        /// <summary>
        /// Останавливает звуки принадлежащего эффекту AudioSource.
        /// </summary>
        protected override void OnStop()
        {
            if (_source != null)
                _source.Stop();
        }
    }
}