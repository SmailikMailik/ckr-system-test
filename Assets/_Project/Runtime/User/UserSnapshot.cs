using Project.Common.Validation;

namespace Project.User
{
    /// <summary>
    /// Хранит неизменяемый согласованный снимок валюты и энергии пользователя.
    /// </summary>
    public sealed class UserSnapshot
    {
        /// <summary>
        /// Текущий неотрицательный баланс валюты.
        /// </summary>
        public int Currency { get; }

        /// <summary>
        /// Текущий неотрицательный запас энергии; при загрузке ограничивается максимумом сессии.
        /// </summary>
        public int Energy { get; }

        /// <summary>
        /// Создаёт снимок с неотрицательными значениями; максимум энергии применяется сессией при загрузке.
        /// </summary>
        public UserSnapshot(int currency, int energy)
        {
            ArgumentGuard.ThrowIfNegative(currency, nameof(currency));
            ArgumentGuard.ThrowIfNegative(energy, nameof(energy));
            Currency = currency;
            Energy = energy;
        }
    }
}