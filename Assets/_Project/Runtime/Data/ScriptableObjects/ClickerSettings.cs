using Project.Data.Clicker;
using UnityEngine;

namespace Project.Data.ScriptableObjects
{
    /// <summary>
    /// Хранит максимум энергии, награду, расход и интервалы кликера в сериализованном Unity asset.
    /// </summary>
    public sealed class ClickerSettings : ScriptableObject
    {
        [SerializeField, Min(1)] private int _maxEnergy = 1000;
        [SerializeField, Min(0)] private int _reward = 1;
        [SerializeField, Min(0)] private int _clickCost = 1;
        [SerializeField, Min(0.1f)] private float _autoCollectSeconds = 3;
        [SerializeField, Min(0.1f)] private float _energyRecoverySeconds = 10;
        [SerializeField, Min(0)] private int _energyRecovery = 10;

        /// <summary>
        /// Создаёт проверенный неизменяемый снимок текущих полей asset.
        /// </summary>
        public ClickerConfig ToConfig()
        {
            return new(
                _reward,
                _clickCost,
                _autoCollectSeconds,
                _energyRecoverySeconds,
                _energyRecovery,
                _maxEnergy);
        }
    }
}