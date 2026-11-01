using Project.UI.Effects.Playback;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Effects.Triggers
{
    /// <summary>
    /// Связывает Button с проигрывателем эффектов только на период активности компонента.
    /// </summary>
    [AddComponentMenu("Project/UI/Effects/Play On Click")]
    internal sealed class EffectOnClick : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private EffectPlayer _player;

        private void OnEnable()
        {
            _button.onClick.AddListener(_player.Play);
        }

        private void OnDisable()
        {
            if (_button != null && _player != null)
                _button.onClick.RemoveListener(_player.Play);

            if (_player != null)
                _player.Stop();
        }
    }
}