using System;
using Project.User;
using UnityEngine;
using Zenject;

namespace Project.Runtime.Composition.Startup
{
    /// <summary>
    /// Сохраняет завершённые изменения пользователя по интервалу и при событиях жизненного цикла приложения.
    /// </summary>
    internal sealed class UserPersistenceLifecycle : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float _autosaveIntervalSeconds = 5;

        private UserDataService _userData;
        private UserSession _session;
        private double _elapsed;

        /// <summary>
        /// Получает глобальные данные для сохранения после успешной инициализации Startup.
        /// </summary>
        [Inject]
        internal void Construct(UserDataService userData, UserSession session)
        {
            _userData = userData;
            _session = session;
        }

        private void Update()
        {
            if (_session.IsInitialized is false)
                return;

            _elapsed += Time.unscaledDeltaTime;

            if (_elapsed < _autosaveIntervalSeconds)
                return;

            _elapsed = 0;
            TrySave();
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
                TrySave();
        }

        private void OnApplicationFocus(bool focused)
        {
            if (focused is false)
                TrySave();
        }

        private void OnApplicationQuit()
        {
            TrySave();
        }

        private void TrySave()
        {
            if (_session.IsInitialized is false)
                return;

            try
            {
                _userData.Save();
            }
            catch (Exception exception)
            {
                Debug.LogError($"[{nameof(UserPersistenceLifecycle)}::{nameof(TrySave)}] User data save failed; pending changes will be retried: {exception}", this);
            }
        }
    }
}