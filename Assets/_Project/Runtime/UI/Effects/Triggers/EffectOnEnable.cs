using Project.UI.Effects.Playback;
using UnityEngine;

namespace Project.UI.Effects.Triggers
{
    /// <summary>
    /// Запускает локальный набор эффектов при включении объекта или компонента и останавливает при отключении.
    /// </summary>
    [AddComponentMenu("Project/UI/Effects/Play On Enable")]
    internal sealed class EffectOnEnable : MonoBehaviour
    {
        [SerializeField] private EffectPlayer _player;

        private void OnEnable()
        {
            _player.Play();
        }

        private void OnDisable()
        {
            if (_player != null)
                _player.Stop();
        }
    }
}