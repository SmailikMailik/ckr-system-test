using System;
using Project.UI.Views;
using Project.User;
using UnityEngine;
using Zenject;

namespace Project.Runtime.Composition.Startup
{
    /// <summary>
    /// Загружает данные пользователя до открытия Main и оставляет доступный повтор при ошибке.
    /// </summary>
    internal sealed class StartupController : MonoBehaviour
    {
        [SerializeField] private StartupView _view;

        private UserDataService _userData;
        private IStartupSceneRouter _router;
        private bool _starting;

        /// <summary>
        /// Получает глобальный сервис данных и выбранный переход между сценами.
        /// </summary>
        [Inject]
        internal void Construct(UserDataService userData, IStartupSceneRouter router)
        {
            _userData = userData;
            _router = router;
        }

        private void Start()
        {
            _view.RetryRequested += TryStart;
            _view.Bind();
            TryStart();
        }

        private void OnDestroy()
        {
            if (_view != null)
            {
                _view.RetryRequested -= TryStart;
                _view.Unbind();
            }
        }

        /// <summary>
        /// Подготавливает пользователя и открывает Main только после успешного чтения сохранения.
        /// </summary>
        internal void TryStart()
        {
            if (_starting)
                return;

            _starting = true;
            _view.ShowLoading();

            try
            {
                _userData.Initialize();
                _router.OpenMain();
            }
            catch (Exception exception)
            {
                _starting = false;
                _view.ShowError();
                Debug.LogError($"[{nameof(StartupController)}::{nameof(TryStart)}] Application startup failed: {exception}", this);
            }
        }
    }
}