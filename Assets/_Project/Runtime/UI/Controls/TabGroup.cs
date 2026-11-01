using System;
using Project.Common.Validation;
using Project.UI.Settings;
using TMPro;
using UnityEngine;

namespace Project.UI.Controls
{
    /// <summary>
    /// Переключает произвольный набор панелей и согласованно оформляет кнопки навигации.
    /// Передаёт запрос выбора вкладки владельцу экрана.
    /// </summary>
    [DisallowMultipleComponent]
    internal sealed class TabGroup : MonoBehaviour
    {
        [SerializeField] private UiToolkitTheme _theme;
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private NavigationTab[] _tabs;

        /// <summary>
        /// Подключает запрос выбора вкладки к общему времени жизни кнопок экрана.
        /// </summary>
        internal void Bind(ButtonBindings bindings, Action<int> select)
        {
            for (var i = 0; i < _tabs.Length; i++)
                bindings.Add(
                    _tabs[i].Button,
                    (Select: select, Index: i),
                    static state => state.Select(state.Index));
        }

        /// <summary>
        /// Показывает ровно одну панель и обновляет заголовок и оформление её кнопки.
        /// </summary>
        internal void ShowTab(int tab)
        {
            ArgumentGuard.ThrowIfOutOfRange(tab, 0, _tabs.Length - 1, nameof(tab));

            _titleLabel.text = _tabs[tab].Title;

            for (var i = 0; i < _tabs.Length; i++)
                _tabs[i].Show(i == tab, _theme);
        }
    }
}