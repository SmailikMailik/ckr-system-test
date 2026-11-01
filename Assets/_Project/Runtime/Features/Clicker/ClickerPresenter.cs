using System;
using Project.Data.Clicker;
using Project.UI.Controls;
using Project.UI.Settings;
using Project.UI.Views;
using Project.User;
using Zenject;

namespace Project.Features.Clicker
{
    /// <summary>
    /// Связывает ручной сбор, изменения ресурсов и эффекты начисления с представлением кликера.
    /// </summary>
    internal sealed class ClickerPresenter : IDisposable
    {
        private readonly ClickerView _view;
        private readonly ClickerConfig _settings;
        private readonly ClickerGame _game;
        private readonly UserSession _user;
        private readonly ButtonBindings _buttons = new();
        private bool _initialized;

        /// <summary>
        /// Получает представление кликера, правила и подготовленную пользовательскую сессию.
        /// </summary>
        [Inject]
        internal ClickerPresenter(ClickerView view, ClickerConfig settings, ClickerGame game, UserSession user)
        {
            _view = view;
            _settings = settings;
            _game = game;
            _user = user;
        }

        /// <summary>
        /// Подключает ручной сбор и обновления ресурсов; повторный вызов сохраняет единственный набор подписок.
        /// </summary>
        internal void Initialize()
        {
            if (_initialized)
                return;

            _view.Bind(_buttons, _game.Collect);
            _user.Changed += RefreshBalances;
            _game.Collected += _view.PlayCollect;
            _initialized = true;
            var culture = UiFormatting.Culture;
            var recoverySeconds = _settings.EnergyRecoverySeconds.ToString("g", culture);
            _view.ShowClickerSettings(
                $"Восстановление: +{_settings.EnergyRecovery} / {recoverySeconds} с",
                $"+{_settings.Reward}");
            RefreshBalances(_user.Snapshot);
        }

        /// <summary>
        /// Снимает только собственные подписки; допускает повторное подключение к тому же представлению.
        /// </summary>
        public void Dispose()
        {
            if (_initialized is false)
                return;

            _initialized = false;
            _user.Changed -= RefreshBalances;
            _game.Collected -= _view.PlayCollect;
            _buttons.Dispose();
        }

        private void RefreshBalances(UserSnapshot snapshot)
        {
            _view.ShowBalances(snapshot.Currency, snapshot.Energy, _user.Energy.MaxEnergy, _game.CanCollect);
        }
    }
}