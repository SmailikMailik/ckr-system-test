using System;
using Project.Common.Validation;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.UI.Controls
{
    /// <summary>
    /// Объединяет uGUI-кнопку, необязательную подпись и заменяемое действие нажатия.
    /// </summary>
    [RequireComponent(typeof(Button))]
    internal sealed class UiButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _label;

        private IDisposable _action;

        internal ColorBlock Colors
        {
            get => _button.colors;
            set => _button.colors = value;
        }

        private void OnDestroy()
        {
            _action?.Dispose();
        }

        /// <summary>
        /// Задаёт текст подписи; операция требует назначенный компонент подписи.
        /// </summary>
        internal void SetText(string text)
        {
            _label.text = text;
        }

        /// <summary>
        /// Задаёт доступность штатного Button, сохраняя его визуальные переходы.
        /// </summary>
        internal void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }

        /// <summary>
        /// Заменяет только собственное действие; освобождение результата отключает именно эту привязку.
        /// </summary>
        internal IDisposable SetAction(Action action)
        {
            ArgumentGuard.ThrowIfNull(action, nameof(action));

            return SetAction(action, static callback => callback());
        }

        /// <summary>
        /// Заменяет собственное действие, передавая обработчику сохранённое состояние до освобождения привязки.
        /// </summary>
        internal IDisposable SetAction<TState>(TState state, Action<TState> action)
        {
            ArgumentGuard.ThrowIfNull(action, nameof(action));

            _action?.Dispose();
            _action = new ClickSubscription<TState>(_button, state, action);
            return _action;
        }

        /// <summary>
        /// Снимает один обработчик и допускает повторное освобождение после уничтожения кнопки.
        /// </summary>
        private sealed class ClickSubscription<TState> : IDisposable
        {
            private readonly Button _button;
            private readonly TState _state;
            private readonly Action<TState> _action;
            private UnityAction _handler;

            /// <summary>
            /// Подключает к кнопке один обработчик с сохранённым состоянием до освобождения подписки.
            /// </summary>
            internal ClickSubscription(Button button, TState state, Action<TState> action)
            {
                _button = button;
                _state = state;
                _action = action;
                _handler = Invoke;
                _button.onClick.AddListener(_handler);
            }

            /// <summary>
            /// Снимает обработчик этой подписки, сохраняя новое действие кнопки.
            /// </summary>
            public void Dispose()
            {
                if (_handler is null)
                    return;

                if (_button != null)
                    _button.onClick.RemoveListener(_handler);

                _handler = null;
            }

            private void Invoke()
            {
                _action(_state);
            }
        }
    }
}