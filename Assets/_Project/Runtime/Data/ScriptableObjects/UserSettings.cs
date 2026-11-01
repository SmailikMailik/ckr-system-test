using Project.User;
using UnityEngine;

namespace Project.Data.ScriptableObjects
{
    /// <summary>
    /// Хранит начальные ресурсы нового пользователя в сериализованном Unity asset.
    /// </summary>
    public sealed class UserSettings : ScriptableObject
    {
        [SerializeField, Min(0)] private int _initialCurrency;
        [SerializeField, Min(0)] private int _initialEnergy = 1000;

        /// <summary>
        /// Создаёт проверенную конфигурацию для загрузки пользовательской сессии.
        /// </summary>
        public UserConfig ToConfig()
        {
            return new(_initialCurrency, _initialEnergy);
        }
    }
}