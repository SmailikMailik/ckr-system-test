using Project.Common.Validation;

namespace Project.Data.Clicker
{
    /// <summary>
    /// Хранит проверенный снимок конфигурации кликера на время игровой сессии.
    /// </summary>
    public sealed class ClickerConfig
    {
        /// <summary>
        /// Игровой максимум энергии, общий для загрузки пользователя и работы кликера.
        /// </summary>
        public int MaxEnergy { get; }

        /// <summary>
        /// Награда за успешное ручное или автоматическое нажатие.
        /// </summary>
        public int Reward { get; }

        /// <summary>
        /// Расход энергии на успешное нажатие.
        /// </summary>
        public int ClickCost { get; }

        /// <summary>
        /// Интервал автоматического сбора в секундах.
        /// </summary>
        public float AutoCollectSeconds { get; }

        /// <summary>
        /// Интервал восстановления энергии в секундах.
        /// </summary>
        public float EnergyRecoverySeconds { get; }

        /// <summary>
        /// Энергия, начисляемая за одно восстановление.
        /// </summary>
        public int EnergyRecovery { get; }

        /// <summary>
        /// Проверяет параметры кликера и сохраняет их для игровой сессии.
        /// </summary>
        public ClickerConfig(
            int reward,
            int clickCost,
            float autoCollectSeconds,
            float energyRecoverySeconds,
            int energyRecovery,
            int maxEnergy)
        {
            ArgumentGuard.ThrowIfNegative(reward, nameof(reward));
            ArgumentGuard.ThrowIfNegative(clickCost, nameof(clickCost));
            ArgumentGuard.ThrowIfNotPositiveFinite(autoCollectSeconds, nameof(autoCollectSeconds));
            ArgumentGuard.ThrowIfNotPositiveFinite(energyRecoverySeconds, nameof(energyRecoverySeconds));
            ArgumentGuard.ThrowIfNegative(energyRecovery, nameof(energyRecovery));

            ArgumentGuard.ThrowIfNotPositive(maxEnergy, nameof(maxEnergy));

            MaxEnergy = maxEnergy;
            Reward = reward;
            ClickCost = clickCost;
            AutoCollectSeconds = autoCollectSeconds;
            EnergyRecoverySeconds = energyRecoverySeconds;
            EnergyRecovery = energyRecovery;
        }
    }
}