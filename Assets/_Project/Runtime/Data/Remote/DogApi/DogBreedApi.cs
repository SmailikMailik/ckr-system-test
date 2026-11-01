using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Project.Common.Validation;
using Project.Data.Breeds;
using Project.Networking;
using Zenject;

namespace Project.Data.Remote.DogApi
{
    /// <summary>
    /// Запрашивает страницы и детали Dog API через общую очередь приложения.
    /// </summary>
    internal sealed class DogBreedApi : IBreedSource
    {
        private const string Endpoint = "https://dogapi.dog/api/v2/breeds";
        private const string AcceptMediaType = "application/json";
        private const int MaximumPageSize = 1000;
        private readonly HttpRequestQueue _queue;

        /// <summary>
        /// Получает общую очередь приложения.
        /// </summary>
        [Inject]
        internal DogBreedApi(HttpRequestQueue queue)
        {
            _queue = queue;
        }

        /// <summary>
        /// Запрашивает страницу указанного размера и возвращает все её записи после проверки формата ответа.
        /// </summary>
        public Task<IReadOnlyList<DogBreed>> GetBreedsAsync(
            int pageNumber,
            int pageSize,
            int timeoutSeconds,
            CancellationToken cancellation)
        {
            ArgumentGuard.ThrowIfNotPositive(pageNumber, nameof(pageNumber));
            ArgumentGuard.ThrowIfOutOfRange(pageSize, 1, MaximumPageSize, nameof(pageSize));

            return _queue.GetAsync(
                $"{Endpoint}?page[number]={pageNumber}&page[size]={pageSize}",
                timeoutSeconds,
                AcceptMediaType,
                DogBreedParser.ParseList,
                cancellation);
        }

        /// <summary>
        /// Получает описание отдельным запросом по идентификатору выбранной породы.
        /// </summary>
        public Task<DogBreed> GetBreedAsync(string id, int timeoutSeconds, CancellationToken cancellation)
        {
            ArgumentGuard.ThrowIfBlank(id, nameof(id));

            return _queue.GetAsync(
                $"{Endpoint}/{Uri.EscapeDataString(id)}",
                timeoutSeconds,
                AcceptMediaType,
                DogBreedParser.ParseDetails,
                cancellation);
        }
    }
}