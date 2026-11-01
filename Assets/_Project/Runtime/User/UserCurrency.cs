using System;
using Project.Common.Mathematics;
using Project.Common.Validation;

namespace Project.User
{
    /// <summary>
    /// Хранит валюту пользователя и проверяет допустимость начислений и списаний.
    /// </summary>
    public sealed class UserCurrency : IUserResource
    {
        /// <summary>
        /// Текущий баланс валюты.
        /// </summary>
        public int Balance { get; private set; }

        /// <summary>
        /// Создаёт баланс с неотрицательным начальным значением.
        /// </summary>
        public UserCurrency(int initialBalance)
        {
            ArgumentGuard.ThrowIfNegative(initialBalance, nameof(initialBalance));
            Balance = initialBalance;
        }

        /// <summary>
        /// Проверяет, что итоговый баланс находится в диапазоне от 0 до int.MaxValue.
        /// </summary>
        public bool CanApply(int delta)
        {
            return Balance.CanAdd(delta) && (Balance + delta).IsNonNegative();
        }

        /// <summary>
        /// Создаёт описание изменения для согласованной операции пользовательской сессии.
        /// </summary>
        public UserResourceChange ChangeBy(int delta)
        {
            return new(this, delta);
        }

        /// <summary>
        /// Применяет проверяемое изменение внутри согласованной операции пользовательской сессии.
        /// </summary>
        internal void Apply(int delta)
        {
            if (CanApply(delta) is false)
                throw new InvalidOperationException("Изменение выходит за допустимые границы валюты.");

            Balance += delta;
        }

        void IUserResource.Apply(int delta)
        {
            Apply(delta);
        }
    }
}