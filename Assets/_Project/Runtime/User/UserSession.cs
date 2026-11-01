using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Project.Common.Validation;

[assembly: InternalsVisibleTo("Project.Tests.EditMode")]

namespace Project.User
{
    /// <summary>
    /// Владеет ресурсами пользователя и публикует снимок только после согласованного изменения.
    /// </summary>
    public sealed class UserSession
    {
        private readonly HashSet<IUserResource> _resources = new();
        private readonly int _maxEnergy;

        /// <summary>
        /// Получает игровой лимит энергии независимо от начального и сохранённого состояния пользователя.
        /// </summary>
        public UserSession(int maxEnergy)
        {
            ArgumentGuard.ThrowIfNotPositive(maxEnergy, nameof(maxEnergy));
            _maxEnergy = maxEnergy;
        }

        /// <summary>
        /// Данные успешно загружены Startup; только после этого доступны операции и снимок.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Валюта текущего пользователя; изменение выполняется через согласованную операцию сессии.
        /// </summary>
        public UserCurrency Currency { get; private set; }

        /// <summary>
        /// Энергия текущего пользователя с игровым максимумом текущей сессии.
        /// </summary>
        public UserEnergy Energy { get; private set; }

        /// <summary>
        /// Публикует один завершённый снимок после фактического изменения ресурсов.
        /// </summary>
        public event Action<UserSnapshot> Changed;

        /// <summary>
        /// Создаёт согласованный снимок подготовленной сессии; до Initialize чтение запрещено.
        /// </summary>
        public UserSnapshot Snapshot
        {
            get
            {
                EnsureInitialized();
                return new(Currency.Balance, Energy.Current);
            }
        }

        /// <summary>
        /// Однократно восстанавливает сохранение или начальные значения; энергию ограничивает текущим максимумом.
        /// </summary>
        public void Initialize(UserConfig config, UserSnapshot snapshot)
        {
            ArgumentGuard.ThrowIfNull(config, nameof(config));

            if (IsInitialized)
                throw new InvalidOperationException("Пользовательская сессия уже подготовлена.");

            ArgumentGuard.ThrowIfOutOfRange(config.InitialEnergy, 0, _maxEnergy, nameof(config.InitialEnergy));

            Currency = new(snapshot?.Currency ?? config.InitialCurrency);
            Energy = new(Math.Min(snapshot?.Energy ?? config.InitialEnergy, _maxEnergy), _maxEnergy);
            _resources.Add(Currency);
            _resources.Add(Energy);
            IsInitialized = true;
        }

        /// <summary>
        /// Проверяет весь набор изменений; повторные изменения одного ресурса суммируются до проверки границ.
        /// </summary>
        public bool CanApply(params UserResourceChange[] changes)
        {
            return TryPrepare(changes, out _);
        }

        /// <summary>
        /// Применяет согласованную операцию: при отказе сохраняет исходные ресурсы, при изменении публикует
        /// один снимок.
        /// </summary>
        public bool TryApply(params UserResourceChange[] changes)
        {
            if (TryPrepare(changes, out var totals) is false)
                return false;

            var changed = false;

            foreach (var pair in totals)
            {
                if (pair.Value == 0)
                    continue;

                pair.Key.Apply((int)pair.Value);
                changed = true;
            }

            if (changed)
                Changed?.Invoke(Snapshot);

            return true;
        }

        /// <summary>
        /// Ограничивает восстановление максимумом и сообщает только о фактическом изменении энергии.
        /// </summary>
        public void RestoreEnergy(int amount)
        {
            EnsureInitialized();
            var previous = Energy.Current;
            Energy.Restore(amount);

            if (previous != Energy.Current)
                Changed?.Invoke(Snapshot);
        }

        private bool TryPrepare(UserResourceChange[] changes, out Dictionary<IUserResource, long> totals)
        {
            EnsureInitialized();
            ArgumentGuard.ThrowIfNull(changes, nameof(changes));
            totals = new();

            foreach (var change in changes)
            {
                if (change.Resource is null || _resources.Contains(change.Resource) is false)
                    throw new ArgumentException("Изменение должно принадлежать ресурсу текущей сессии.", nameof(changes));

                totals.TryGetValue(change.Resource, out var total);
                totals[change.Resource] = total + change.Delta;
            }

            foreach (var pair in totals)
            {
                if (pair.Value < int.MinValue || pair.Value > int.MaxValue
                    || pair.Key.CanApply((int)pair.Value) is false)
                    return false;
            }

            return true;
        }

        private void EnsureInitialized()
        {
            if (IsInitialized is false)
                throw new InvalidOperationException("Пользовательская сессия ещё не подготовлена Startup.");
        }
    }
}