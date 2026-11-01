using System;
using System.Linq;
using Project.Common.Validation;

namespace Project.Data.Remote.Nws
{
    /// <summary>
    /// Задаёт точку сетки NWS; проверяет формат office ID и неотрицательные координаты.
    /// </summary>
    internal sealed class NwsGridpoint
    {
        private const int OfficeIdLength = 3;

        internal string OfficeId { get; }
        internal int X { get; }
        internal int Y { get; }

        /// <summary>
        /// Проверяет локальный формат точки; наличие office и ячейки в данных определяет сервис NWS.
        /// </summary>
        internal NwsGridpoint(string officeId, int gridX, int gridY)
        {
            ArgumentGuard.ThrowIfBlank(officeId, nameof(officeId));

            if (officeId.Length != OfficeIdLength
                || officeId.All(character => (character >= 'A' && character <= 'Z')
                    || (character >= '0' && character <= '9')) is false)
                throw new ArgumentException("Нужен трёхсимвольный office ID NWS в верхнем регистре.", nameof(officeId));

            ArgumentGuard.ThrowIfNegative(gridX, nameof(gridX));
            ArgumentGuard.ThrowIfNegative(gridY, nameof(gridY));

            OfficeId = officeId;
            X = gridX;
            Y = gridY;
        }
    }
}