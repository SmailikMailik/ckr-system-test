using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Project.Runtime.Composition.Startup
{
    /// <summary>
    /// Открывает Main через Zenject, сохраняя глобальные зависимости пользователя.
    /// </summary>
    internal sealed class ZenjectStartupSceneRouter : IStartupSceneRouter
    {
        private readonly string _mainSceneName;
        private readonly ZenjectSceneLoader _loader;

        /// <summary>
        /// Получает загрузчик текущего SceneContext и имя главной сцены из композиции.
        /// </summary>
        [Inject]
        internal ZenjectStartupSceneRouter(ZenjectSceneLoader loader, string mainSceneName)
        {
            _loader = loader;
            _mainSceneName = mainSceneName;
        }

        /// <summary>
        /// Запускает асинхронную замену Startup на Main.
        /// </summary>
        public void OpenMain()
        {
            // Zenject уничтожает текущие SceneContext до собственной проверки доступности новой сцены.
            if (Application.CanStreamedLevelBeLoaded(_mainSceneName) is false)
                throw new InvalidOperationException($"Главная сцена {_mainSceneName} недоступна для загрузки.");

            _loader.LoadSceneAsync(_mainSceneName, LoadSceneMode.Single);
        }
    }
}