using Project.Common.Validation;

namespace Project.User
{
    /// <summary>
    /// Хранит начальные ресурсы нового пользователя.
    /// </summary>
    public sealed class UserConfig
    {
        /// <summary>
        /// Неотрицательная начальная валюта нового пользователя.
        /// </summary>
        public int InitialCurrency { get; }

        /// <summary>
        /// Неотрицательная начальная энергия нового пользователя.
        /// </summary>
        public int InitialEnergy { get; }

        /// <summary>
        /// Проверяет начальное состояние до загрузки пользовательской сессии.
        /// </summary>
        public UserConfig(int initialCurrency, int initialEnergy)
        {
            ArgumentGuard.ThrowIfNegative(initialCurrency, nameof(initialCurrency));
            ArgumentGuard.ThrowIfNegative(initialEnergy, nameof(initialEnergy));
            InitialCurrency = initialCurrency;
            InitialEnergy = initialEnergy;
        }
    }
}