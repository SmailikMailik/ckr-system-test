using System;
using System.Runtime.CompilerServices;
using Project.UI.Controls;
using Project.UI.Settings;
using TMPro;
using UnityEngine;

[assembly: InternalsVisibleTo("Project.Runtime")]
[assembly: InternalsVisibleTo("Project.Editor")]
[assembly: InternalsVisibleTo("Project.Features.Clicker")]
[assembly: InternalsVisibleTo("Project.Features.Weather")]
[assembly: InternalsVisibleTo("Project.Features.Breeds")]
[assembly: InternalsVisibleTo("Project.Tests.EditMode")]
[assembly: InternalsVisibleTo("Project.Tests.PlayMode")]

namespace Project.UI.Views
{
    /// <summary>
    /// Объединяет представления главного экрана и передаёт выбор вкладки в MainPresenter.
    /// </summary>
    internal sealed class MainView : MonoBehaviour
    {
        [SerializeField] private TabGroup _navigation;
        [SerializeField] private ClickerView _clicker;
        [SerializeField] private WeatherView _weather;
        [SerializeField] private BreedsView _breeds;
        [SerializeField] private InformationModalView _informationModal;
        [SerializeField] private TMP_Text _headerCurrencyLabel;

        private readonly ButtonBindings _buttons = new();
        private bool _isBound;

        internal ClickerView Clicker => _clicker;
        internal WeatherView Weather => _weather;
        internal BreedsView Breeds => _breeds;
        internal InformationModalView InformationModal => _informationModal;

        /// <summary>
        /// Запрашивает открытие вкладки по её индексу в нижней навигации.
        /// </summary>
        internal event Action<int> TabRequested;

        private void OnDestroy()
        {
            Unbind();
        }

        /// <summary>
        /// Подключает навигацию; повторный вызов сохраняет единственный набор подписок.
        /// </summary>
        internal void Bind()
        {
            if (_isBound)
                return;

            _weather.Initialize();
            _navigation.Bind(_buttons, tab => TabRequested?.Invoke(tab));
            _isBound = true;
        }

        /// <summary>
        /// Снимает только обработчики экрана, сохраняя сторонние listeners кнопок.
        /// </summary>
        internal void Unbind()
        {
            _buttons.Dispose();
            _isBound = false;
        }

        /// <summary>
        /// Показывает выбранный экран, заголовок и согласованное активное состояние нижней навигации.
        /// </summary>
        internal void ShowTab(int tab)
        {
            _navigation.ShowTab(tab);
        }

        /// <summary>
        /// Обновляет общий счётчик валюты независимо от представления текущей вкладки.
        /// </summary>
        internal void ShowCurrency(int currency)
        {
            _headerCurrencyLabel.text = currency.ToString("N0", UiFormatting.Culture);
        }

        /// <summary>
        /// Закрывает общее всплывающее окно, сохраняя состояние остальных экранов.
        /// </summary>
        internal void HideModal()
        {
            _informationModal.HideModal();
        }

        /// <summary>
        /// Убирает индикаторы при смене владельца запросов.
        /// </summary>
        internal void ClearLoading()
        {
            _weather.ClearLoading();
            _breeds.ClearLoading();
        }
    }
}