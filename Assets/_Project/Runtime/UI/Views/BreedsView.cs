using System;
using System.Collections.Generic;
using Project.UI.Controls;
using TMPro;
using UnityEngine;

namespace Project.UI.Views
{
    /// <summary>
    /// Отображает список через экземпляры prefab строки, повторно используя их до уничтожения экрана.
    /// </summary>
    [DisallowMultipleComponent]
    internal sealed class BreedsView : MonoBehaviour
    {
        [SerializeField] private BreedRowView _rowPrefab;
        [SerializeField] private RectTransform _content;
        [SerializeField] private TMP_Text _statusLabel;
        [SerializeField] private GameObject _loadingIndicator;
        [SerializeField] private TMP_Text _listEndLabel;

        private const int NoLoadingBreedIndex = -1;
        private readonly List<BreedRowView> _rows = new();
        private readonly ButtonBindings _buttons = new();
        private Action<int> _select;

        private void OnDestroy()
        {
            Unbind();
        }

        /// <summary>
        /// Подключает выбор существующих и будущих строк в пределах времени жизни этого view.
        /// </summary>
        internal void Bind(Action<int> select)
        {
            Unbind();
            _select = select;

            for (var i = 0; i < _rows.Count; i++)
                BindRow(i);
        }

        /// <summary>
        /// Снимает собственные действия строк, сохраняя экземпляры для следующего подключения.
        /// </summary>
        internal void Unbind()
        {
            _buttons.Dispose();
            _select = null;
        }

        /// <summary>
        /// Создаёт недостающие строки из prefab, обновляет список и скрывает лишние строки для повторного использования.
        /// </summary>
        internal void ShowBreeds(IReadOnlyList<string> breeds, IReadOnlyList<string> ids = null)
        {
            while (_rows.Count < breeds.Count)
            {
                var row = Instantiate(_rowPrefab, _content);
                row.transform.SetSiblingIndex(_listEndLabel.transform.GetSiblingIndex());
                row.ShowLoading(false);
                _rows.Add(row);

                if (_select is not null)
                    BindRow(_rows.Count - 1);
            }

            _listEndLabel.gameObject.SetActive(breeds.Count > 0);
            _listEndLabel.text = $"Показано пород: {breeds.Count}";

            for (var i = 0; i < _rows.Count; i++)
            {
                var visible = i < breeds.Count;
                var id = visible && ids is not null && i < ids.Count ? ids[i] : null;
                _rows[i].Show(visible, visible ? breeds[i] : string.Empty, id);
            }
        }

        /// <summary>
        /// Отображает загрузку списка, пустой ответ или ошибку.
        /// </summary>
        internal void ShowBreedsStatus(string status, bool loading)
        {
            _statusLabel.text = status;
            _statusLabel.gameObject.SetActive(string.IsNullOrEmpty(status) is false);
            _loadingIndicator.SetActive(loading);
        }

        /// <summary>
        /// Показывает индикатор только на выбранной строке.
        /// </summary>
        internal void ShowBreedLoading(int index)
        {
            for (var i = 0; i < _rows.Count; i++)
                _rows[i].ShowLoading(i == index);
        }

        /// <summary>
        /// Снимает индикаторы списка и выбранной породы.
        /// </summary>
        internal void ClearLoading()
        {
            _loadingIndicator.SetActive(false);
            ShowBreedLoading(NoLoadingBreedIndex);
        }

        private void BindRow(int index)
        {
            _buttons.Add(_rows[index].Button, (Select: _select, Index: index), static state => state.Select(state.Index));
        }
    }
}