using Project.UI.Effects.Playback;
using UnityEngine;

namespace Project.UI.Effects.Feedback
{
    /// <summary>
    /// Передаёт факт начисления общим эффектам, а значение награды — специализированному представлению полётов.
    /// </summary>
    internal sealed class ClickerFeedback : MonoBehaviour
    {
        [SerializeField] private EffectPlayer _effects;
        [SerializeField] private RewardFlightsEffect _rewardFlights;

        private void OnDisable()
        {
            _effects.Stop();
            _rewardFlights.Stop();
        }

        /// <summary>
        /// Запускает настроенные в prefab эффекты начисления и передаёт им размер награды.
        /// </summary>
        internal void Play(int reward)
        {
            if (isActiveAndEnabled is false)
                return;

            _effects.Play();
            _rewardFlights.Play(reward);
        }
    }
}