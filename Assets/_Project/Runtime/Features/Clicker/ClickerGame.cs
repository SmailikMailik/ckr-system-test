using System;
using System.Runtime.CompilerServices;
using Project.Common.Validation;
using Project.Data.Clicker;
using Project.User;
using UnityEngine;
using Zenject;

[assembly: InternalsVisibleTo("Project.Runtime")]
[assembly: InternalsVisibleTo("Project.Tests.EditMode")]
[assembly: InternalsVisibleTo("Project.Tests.PlayMode")]

namespace Project.Features.Clicker
{
    /// <summary>
    /// Применяет правила сбора и ведёт таймеры кликера независимо от текущей вкладки.
    /// </summary>
    public sealed class ClickerGame : ITickable
    {
        private readonly ClickerConfig _settings;
        private readonly UserResourceChange[] _collectionChanges;
        private double _autoElapsed;
        private double _recoveryElapsed;

        /// <summary>
        /// Баланс валюты текущей сессии.
        /// </summary>
        public UserCurrency Currency => User.Currency;

        /// <summary>
        /// Запас энергии текущей сессии.
        /// </summary>
        public UserEnergy Energy => User.Energy;

        /// <summary>
        /// Подготовленная пользовательская сессия, независимая от времени жизни сцены.
        /// </summary>
        public UserSession User { get; }

        /// <summary>
        /// Доступен ли сбор с учётом энергии и допустимого значения баланса.
        /// </summary>
        public bool CanCollect => User.CanApply(_collectionChanges);

        /// <summary>
        /// Сообщает об успешном сборе и передаёт размер начисленной награды.
        /// </summary>
        public event Action<int> Collected;

        /// <summary>
        /// Получает правила кликера и ресурсы текущей сессии.
        /// </summary>
        [Inject]
        public ClickerGame(ClickerConfig settings, UserSession user)
        {
            _settings = settings;
            User = user;
            _collectionChanges = new[]
            {
                user.Currency.ChangeBy(settings.Reward),
                user.Energy.ChangeBy(-settings.ClickCost)
            };
        }

        /// <summary>
        /// Продвигает игровые часы в активном приложении, независимо от Time.timeScale.
        /// </summary>
        public void Tick()
        {
            Advance(Time.unscaledDeltaTime);
        }

        /// <summary>
        /// Начисляет награду и списывает энергию при достаточном запасе и допустимом итоговом балансе; ручной и
        /// автоматический сбор выполняются по общим правилам.
        /// </summary>
        public void Collect()
        {
            if (User.TryApply(_collectionChanges) is false)
                return;

            Collected?.Invoke(_settings.Reward);
        }

        /// <summary>
        /// Обрабатывает события в хронологическом порядке, сохраняя остаток обоих таймеров.
        /// </summary>
        internal void Advance(double seconds)
        {
            ArgumentGuard.ThrowIfNegativeOrNotFinite(seconds, nameof(seconds));

            while (seconds > 0)
            {
                var step = GetNextTimerStep(seconds);
                _autoElapsed += step;
                _recoveryElapsed += step;
                seconds -= step;

                // На совпавшей границе сначала восстанавливаем энергию, затем выполняем автосбор.
                if (_recoveryElapsed >= _settings.EnergyRecoverySeconds)
                {
                    _recoveryElapsed = 0;
                    User.RestoreEnergy(_settings.EnergyRecovery);
                }

                if (_autoElapsed >= _settings.AutoCollectSeconds)
                {
                    _autoElapsed = 0;
                    Collect();
                }
            }
        }

        /// <summary>
        /// Ограничивает шаг ближайшей границей таймера и отклоняет время, для которого double теряет прогресс.
        /// </summary>
        private double GetNextTimerStep(double seconds)
        {
            var untilAutoCollect = _settings.AutoCollectSeconds - _autoElapsed;
            var untilRecovery = _settings.EnergyRecoverySeconds - _recoveryElapsed;
            var step = Math.Min(seconds, Math.Min(untilAutoCollect, untilRecovery));

            if (seconds - step >= seconds)
                throw new ArgumentOutOfRangeException(
                    nameof(seconds),
                    "Шаг таймера слишком мал относительно оставшегося времени.");

            return step;
        }
    }
}