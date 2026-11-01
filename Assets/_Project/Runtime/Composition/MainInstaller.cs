using Project.Data.Breeds;
using Project.Data.Remote.DogApi;
using Project.Data.Remote.Nws;
using Project.Data.ScriptableObjects;
using Project.Data.Weather;
using Project.Features.Breeds;
using Project.Features.Clicker;
using Project.Features.Weather;
using Project.Runtime.Presentation.Main;
using Project.UI.Settings;
using Project.UI.Views;
using UnityEngine;
using Zenject;

namespace Project.Runtime.Composition
{
    /// <summary>
    /// Регистрирует зависимости контекста Main и их освобождение.
    /// </summary>
    internal sealed class MainInstaller : MonoInstaller
    {
        [SerializeField] private MainView _view;
        [SerializeField] private WeatherSettings _weatherSettings;
        [SerializeField] private WeatherIconCatalog _weatherIcons;

        private const int RequestTimeoutSeconds = 20;
        private const string TopekaOfficeId = "TOP";
        private const int TopekaGridX = 32;
        private const int TopekaGridY = 81;

        /// <summary>
        /// Регистрирует представление, настройки и системы сцены Main.
        /// </summary>
        public override void InstallBindings()
        {
            // Подписки и освобождение ресурсов сцены.
            Container.BindR3Lifetime();

            // Представление сцены.
            Container.BindInstance(_view);
            Container.BindInstance(_view.Clicker);
            Container.BindInstance(_view.Weather);
            Container.BindInstance(_view.Breeds);
            Container.BindInstance(_view.InformationModal);

            // Игровая конфигурация и визуальные настройки.
            Container.Bind<WeatherPollingConfig>().FromInstance(_weatherSettings.ToConfig());
            Container.BindInstance(_weatherIcons);

            // Получение данных внешних API.
            Container.Bind<IWeatherSource>().To<NwsWeatherApi>().AsSingle()
                .WithArguments(new NwsGridpoint(TopekaOfficeId, TopekaGridX, TopekaGridY));
            Container.Bind<IBreedSource>().To<DogBreedApi>().AsSingle();

            // Правила и таймеры сцены используют подготовленную глобальную сессию.
            Container.BindInterfacesAndSelfTo<ClickerGame>().AsSingle();

            // Посредники вкладок получают собственные зависимости и общую политику timeout.
            Container.Bind<ClickerPresenter>().AsSingle();
            Container.Bind<WeatherTabPresenter>().AsSingle().WithArguments(RequestTimeoutSeconds);
            Container.Bind<BreedsTabPresenter>().AsSingle().WithArguments(RequestTimeoutSeconds);

            // Связь UI с игровыми системами.
            Container.BindInterfacesAndSelfTo<MainPresenter>().AsSingle();
        }
    }
}