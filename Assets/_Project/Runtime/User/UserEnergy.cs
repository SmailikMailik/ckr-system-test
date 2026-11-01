using System;
using Project.Common.Mathematics;
using Project.Common.Validation;

namespace Project.User
{
    /// <summary>
    /// Хранит текущий запас и максимум энергии пользователя, проверяет допустимость изменений.
    /// </summary>
    public sealed class UserEnergy : IUserResource
    {
        private const int MinEnergy = 0;

        /// <summary>
        /// Доступное количество энергии.
        /// </summary>
        public int Current { get; private set; }

        /// <summary>
        /// Максимальное количество энергии текущей сессии.
        /// </summary>
        public int MaxEnergy { get; }

        /// <summary>
        /// Создаёт запас с проверенным текущим значением и положительным максимумом.
        /// </summary>
        public UserEnergy(int current, int maxEnergy)
        {
            ArgumentGuard.ThrowIfNotPositive(maxEnergy, nameof(maxEnergy));
            ArgumentGuard.ThrowIfOutOfRange(current, MinEnergy, maxEnergy, nameof(current));
            MaxEnergy = maxEnergy;
            Current = current;
        }

        /// <summary>
        /// Проверяет, что итоговая энергия находится в диапазоне от 0 до максимума запаса.
        /// </summary>
        public bool CanApply(int delta)
        {
            return Current.CanAdd(delta) && (Current + delta).IsInRange(MinEnergy, MaxEnergy);
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
                throw new InvalidOperationException("Изменение выходит за допустимые границы энергии.");

            Current += delta;
        }

        /// <summary>
        /// Восстанавливает неотрицательное количество энергии, ограничивая результат максимумом.
        /// </summary>
        internal void Restore(int amount)
        {
            ArgumentGuard.ThrowIfNegative(amount, nameof(amount));
            Current = Current.AddClamped(amount, MinEnergy, MaxEnergy);
        }

        void IUserResource.Apply(int delta)
        {
            Apply(delta);
        }
    }
}