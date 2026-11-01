using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Project.Data.Breeds
{
    /// <summary>
    /// Предоставляет страницу пород и описание выбранной породы.
    /// </summary>
    public interface IBreedSource
    {
        /// <summary>
        /// Возвращает запрошенную страницу пород; размер страницы задаёт потребитель.
        /// </summary>
        /// <param name="pageNumber">Номер страницы от единицы.</param>
        /// <param name="pageSize">Количество записей страницы в пределах выбранного источника.</param>
        /// <param name="timeoutSeconds">Положительный timeout операции источника в секундах; HTTP-адаптер применяет его к активному запросу.</param>
        /// <param name="cancellation">Отмена запроса.</param>
        Task<IReadOnlyList<DogBreed>> GetBreedsAsync(
            int pageNumber,
            int pageSize,
            int timeoutSeconds,
            CancellationToken cancellation);

        /// <summary>
        /// Запрашивает описание по идентификатору выбранной породы.
        /// </summary>
        /// <param name="id">Идентификатор породы в выбранном источнике.</param>
        /// <param name="timeoutSeconds">Положительный timeout операции источника в секундах; HTTP-адаптер применяет его к активному запросу.</param>
        /// <param name="cancellation">Отмена запроса.</param>
        Task<DogBreed> GetBreedAsync(string id, int timeoutSeconds, CancellationToken cancellation);
    }
}