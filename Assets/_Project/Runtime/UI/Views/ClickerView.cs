using System;
using Project.UI.Controls;
using Project.UI.Effects.Feedback;
using Project.UI.Settings;
using TMPro;
using UnityEngine;

namespace Project.UI.Views
{
    /// <summary>
    /// Отображает баланс, энергию, настройки и эффекты кликера.
    /// </summary>
    [DisallowMultipleComponent]
    internal sealed class ClickerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _currencyLabel;
        [SerializeField] private TMP_Text _energyLabel;
        [SerializeField] private UiBar _energyBar;
        [SerializeField] private UiButton _collectButton;
        [SerializeField] private TMP_Text _recoveryLabel;
        [SerializeField] private TMP_Text _rewardLabel;
        [SerializeField] private ClickerFeedback _collectFeedback;

        /// <summary>
        /// Подключает ручной сбор к общему владельцу подписок экрана.
        /// </summary>
        internal void Bind(ButtonBindings bindings, Action collect)
        {
            bindings.Add(_collectButton, collect);
        }

        /// <summary>
        /// Обновляет счётчик валюты, энергию, заполнение индикатора и доступность кнопки получения.
        /// </summary>
        internal void ShowBalances(int currency, int energy, int maxEnergy, bool canCollect)
        {
            var culture = UiFormatting.Culture;
            _currencyLabel.text = currency.ToString("N0", culture);
            _energyLabel.text = $"{energy.ToString("N0", culture)} / {maxEnergy.ToString("N0", culture)}";
            _energyBar.SetValue(energy, maxEnergy);
            _collectButton.SetInteractable(canCollect);
        }

        /// <summary>
        /// Обновляет подписи награды и восстановления энергии.
        /// </summary>
        internal void ShowClickerSettings(string recovery, string reward)
        {
            _recoveryLabel.text = recovery;
            _rewardLabel.text = reward;
        }

        /// <summary>
        /// Проигрывает общий эффект ручного и автоматического сбора.
        /// </summary>
        internal void PlayCollect(int reward)
        {
            _collectFeedback.Play(reward);
        }
    }
}