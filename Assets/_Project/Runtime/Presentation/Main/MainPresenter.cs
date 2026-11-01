using System;
using Project.Features.Breeds;
using Project.Features.Clicker;
using Project.Features.Weather;
using Project.UI.Views;
using Project.User;
using UnityEngine;
using Zenject;

namespace Project.Runtime.Presentation.Main
{
    /// <summary>
    /// Управляет навигацией Main, временем жизни посредников вкладок и общим счётчиком валюты.
    /// </summary>
    internal sealed class MainPresenter : IInitializable, ITickable, IDisposable
    {
        private const int NoSelectedTabIndex = -1;
        private const int ClickerTabIndex = 0;
        private const int WeatherTabIndex = 1;
        private const int BreedsTabIndex = 2;
        private readonly MainView _view;
        private readonly ClickerPresenter _clicker;
        private readonly UserSession _user;
        private readonly WeatherTabPresenter _weatherTab;
        private readonly BreedsTabPresenter _breedsTab;
        private bool _initialized;
        private int _tab = NoSelectedTabIndex;

        /// <summary>
        /// Получает общий экран, пользовательскую сессию и подготовленных посредников вкладок.
        /// </summary>
        [Inject]
        internal MainPresenter(
            MainView view,
            ClickerPresenter clicker,
            UserSession user,
            WeatherTabPresenter weatherTab,
            BreedsTabPresenter breedsTab)
        {
            _view = view;
            _clicker = clicker;
            _user = user;
            _weatherTab = weatherTab;
            _breedsTab = breedsTab;
        }

        /// <summary>
        /// Подключает обработчики интерфейса и открывает кликер; запросы запускаются при выборе сетевой
        /// вкладки.
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
                return;

            _view.Bind();
            _view.TabRequested += SelectTab;
            _breedsTab.Initialize();
            _user.Changed += RefreshCurrency;
            _clicker.Initialize();
            _initialized = true;
            SelectTab(ClickerTabIndex);
            RefreshCurrency(_user.Snapshot);
        }

        /// <summary>
        /// Передаёт время активного приложения посреднику погодной вкладки.
        /// </summary>
        public void Tick()
        {
            if (_initialized)
                _weatherTab.Tick(Time.unscaledDeltaTime);
        }

        /// <summary>
        /// Отменяет запросы вкладок и освобождает обработчики при завершении работы посредника.
        /// </summary>
        public void Dispose()
        {
            if (_initialized is false)
                return;

            _initialized = false;
            _weatherTab.Exit();
            _breedsTab.Dispose();
            _tab = NoSelectedTabIndex;
            _user.Changed -= RefreshCurrency;
            _clicker.Dispose();
            _view.TabRequested -= SelectTab;

            if (_view != null)
                _view.Unbind();
        }

        private void SelectTab(int tab)
        {
            if (_tab == tab)
                return;

            _weatherTab.Exit();
            _breedsTab.Exit();
            _tab = tab;
            _view.HideModal();
            _view.ClearLoading();
            _view.ShowTab(tab);

            if (tab == WeatherTabIndex)
                _weatherTab.Enter();
            else if (tab == BreedsTabIndex)
                _breedsTab.Enter();
        }

        private void RefreshCurrency(UserSnapshot snapshot)
        {
            _view.ShowCurrency(snapshot.Currency);
        }
    }
}