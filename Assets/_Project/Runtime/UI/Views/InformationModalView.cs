using System;
using Project.UI.Controls;
using Project.UI.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Views
{
    /// <summary>
    /// Показывает информационное модальное окно с заголовком, текстом и прокруткой.
    /// </summary>
    [DisallowMultipleComponent]
    internal sealed class InformationModalView : MonoBehaviour
    {
        [SerializeField] private BreedPortraitCatalog _portraits;
        [SerializeField] private Image _portrait;
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private TMP_Text _messageLabel;
        [SerializeField] private ScrollRect _contentScroll;
        [SerializeField] private UiButton[] _closeButtons;

        /// <summary>
        /// Подключает закрытие окна и уведомление владельца после закрытия.
        /// </summary>
        internal void Bind(ButtonBindings bindings, Action closed)
        {
            foreach (var button in _closeButtons)
                bindings.Add(button, () =>
                {
                    HideModal();
                    closed();
                });
        }

        /// <summary>
        /// Открывает общее всплывающее окно и возвращает описание в начало прокрутки.
        /// </summary>
        internal void ShowModal(string title, string message, string breedId = null)
        {
            _portrait.sprite = _portraits.Get(breedId);

            _titleLabel.text = title;
            _messageLabel.text = message;
            gameObject.SetActive(true);
            _contentScroll.ResetToTop();
        }

        /// <summary>
        /// Закрывает общее всплывающее окно, сохраняя состояние остальных экранов.
        /// </summary>
        internal void HideModal()
        {
            gameObject.SetActive(false);
        }
    }
}