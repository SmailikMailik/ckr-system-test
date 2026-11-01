using System;
using System.Collections.Generic;

namespace Project.UI.Controls
{
    /// <summary>
    /// Владеет обработчиками кнопок и снимает только собственные подписки, сохраняя чужие listeners.
    /// </summary>
    internal sealed class ButtonBindings : IDisposable
    {
        private readonly List<IDisposable> _bindings = new();

        /// <summary>
        /// Подключает действие к назначенной кнопке до следующего освобождения подписок.
        /// </summary>
        internal void Add(UiButton button, Action handler)
        {
            _bindings.Add(button.SetAction(handler));
        }

        /// <summary>
        /// Передаёт обработчику явное состояние и сохраняет подписку до освобождения владельца.
        /// </summary>
        internal void Add<TState>(UiButton button, TState state, Action<TState> handler)
        {
            _bindings.Add(button.SetAction(state, handler));
        }

        /// <summary>
        /// Снимает обработчики, в том числе при частично уничтоженном UI; допускает повторное подключение.
        /// </summary>
        public void Dispose()
        {
            foreach (var binding in _bindings)
                binding.Dispose();

            _bindings.Clear();
        }
    }
}