using System.IO;
using System.Runtime.CompilerServices;
using Project.Data.Clicker;
using Project.Data.ScriptableObjects;
using Project.Networking;
using Project.User;
using Project.User.Storage;
using UnityEngine;
using Zenject;

// Инструменты редактора настраивают ресурсы, а тесты проверяют контракты и время жизни объектов.
[assembly: InternalsVisibleTo("Project.Editor")]
[assembly: InternalsVisibleTo("Project.Tests.PlayMode")]
[assembly: InternalsVisibleTo("Project.Tests.EditMode")]

namespace Project.Runtime.Composition
{
    /// <summary>
    /// Регистрирует зависимости глобального контекста приложения и их освобождение.
    /// </summary>
    internal sealed class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private UserSettings _userSettings;
        [SerializeField] private ClickerSettings _clickerSettings;

        private const string UserSaveFileName = "user.json";

        /// <summary>
        /// Регистрирует зависимости с общим временем жизни приложения.
        /// </summary>
        public override void InstallBindings()
        {
            // Подписки и освобождение ресурсов приложения.
            Container.BindR3Lifetime();

            // Пользовательская сессия загружается Startup до открытия Main.
            var clickerConfig = _clickerSettings.ToConfig();
            Container.Bind<ClickerConfig>().FromInstance(clickerConfig);
            Container.Bind<UserConfig>().FromInstance(_userSettings.ToConfig());
            Container.Bind<UserSession>().AsSingle().WithArguments(clickerConfig.MaxEnergy);
            Container.Bind<IUserDataStore>().To<JsonUserDataStore>().AsSingle()
                .WithArguments(Path.Combine(UnityEngine.Application.persistentDataPath, UserSaveFileName));
            Container.BindInterfacesAndSelfTo<UserDataService>().AsSingle();

            // HTTP-транспорт и общая последовательная очередь запросов.
            Container.Bind<IHttpTransport>().To<UnityHttpTransport>().AsSingle()
                .WithArguments("CKR-System-Test/1.0");
            Container.BindInterfacesAndSelfTo<HttpRequestQueue>().AsSingle();
        }
    }
}