using Project.UI.Controls;
using Project.UI.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Views
{
    /// <summary>
    /// Отображает одну строку списка и локальное состояние загрузки её описания.
    /// </summary>
    [DisallowMultipleComponent]
    internal sealed class BreedRowView : MonoBehaviour
    {
        [SerializeField] private UiButton _button;
        [SerializeField] private BreedPortraitCatalog _portraits;
        [SerializeField] private Image _portrait;
        [SerializeField] private GameObject _loadingIndicator;
        [SerializeField] private Image _chevronIcon;

        internal UiButton Button => _button;

        /// <summary>
        /// Обновляет видимость, портрет и название переиспользуемой строки.
        /// </summary>
        internal void Show(bool visible, string breedName, string breedId = null)
        {
            gameObject.SetActive(visible);
            _portrait.sprite = visible ? _portraits.Get(breedId) : null;

            ShowLoading(false);

            if (visible is false)
                return;

            _button.SetText(breedName);
        }

        /// <summary>
        /// Согласованно переключает индикатор и стрелку выбранной строки.
        /// </summary>
        internal void ShowLoading(bool loading)
        {
            _loadingIndicator.SetActive(loading);
            _chevronIcon.enabled = loading is false;
        }
    }
}