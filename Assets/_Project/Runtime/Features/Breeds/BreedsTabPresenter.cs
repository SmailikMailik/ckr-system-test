using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Project.Data.Breeds;
using Project.UI.Controls;
using Project.UI.Views;
using UnityEngine;
using Zenject;

[assembly: InternalsVisibleTo("Project.Runtime")]
[assembly: InternalsVisibleTo("Project.Tests.PlayMode")]

namespace Project.Features.Breeds
{
    /// <summary>
    /// Подключает действия списка и окна деталей, загружает породы и владеет отменой запросов.
    /// </summary>
    internal sealed class BreedsTabPresenter : IDisposable
    {
        private const int PageSize = 10;
        private const int FirstPageNumber = 1;
        private const int NoLoadingBreedIndex = -1;
        private readonly BreedsView _view;
        private readonly InformationModalView _modal;
        private readonly IBreedSource _breeds;
        private readonly int _timeoutSeconds;
        private readonly ButtonBindings _buttons = new();
        private IReadOnlyList<DogBreed> _breedList = Array.Empty<DogBreed>();
        private bool _initialized;
        private CancellationTokenSource _cancellation;
        private CancellationTokenSource _detailCancellation;

        /// <summary>
        /// Получает представления списка и окна, источник пород и timeout сетевых операций.
        /// </summary>
        [Inject]
        internal BreedsTabPresenter(
            BreedsView view,
            InformationModalView modal,
            IBreedSource breeds,
            int timeoutSeconds)
        {
            _view = view;
            _modal = modal;
            _breeds = breeds;
            _timeoutSeconds = timeoutSeconds;
        }

        /// <summary>
        /// Подключает действия списка и окна один раз на время жизни посредника.
        /// </summary>
        internal void Initialize()
        {
            if (_initialized)
                return;

            _view.Bind(OpenBreed);
            _modal.Bind(_buttons, CancelDetails);
            _initialized = true;
        }

        /// <summary>
        /// Запрашивает первую страницу заново при каждом входе на вкладку.
        /// </summary>
        internal void Enter()
        {
            Exit();
            _cancellation = new();
            _ = LoadListAsync(_cancellation.Token);
        }

        /// <summary>
        /// Отменяет загрузку списка и деталей; поздние ответы не открывают окно поверх другой вкладки.
        /// </summary>
        internal void Exit()
        {
            Cancel(ref _detailCancellation);
            Cancel(ref _cancellation);
        }

        /// <summary>
        /// Отменяет запросы и снимает собственные обработчики; допускает повторное подключение.
        /// </summary>
        public void Dispose()
        {
            Exit();

            if (_initialized is false)
                return;

            _initialized = false;
            _buttons.Dispose();

            if (_view != null)
                _view.Unbind();
        }

        /// <summary>
        /// Загружает детали выбранной строки, отменяя предыдущий выбор и сохраняя доступность списка.
        /// </summary>
        private void OpenBreed(int index)
        {
            if (_cancellation is null || index < 0 || index >= _breedList.Count)
                return;

            Cancel(ref _detailCancellation);
            _detailCancellation = CancellationTokenSource.CreateLinkedTokenSource(_cancellation.Token);
            _modal.HideModal();
            _view.ShowBreedsStatus(string.Empty, false);
            _view.ShowBreedLoading(index);
            _ = LoadDetailsAsync(_breedList[index].Id, _detailCancellation.Token);
        }

        /// <summary>
        /// Отменяет загрузку описания и убирает индикатор выбранной строки.
        /// </summary>
        private void CancelDetails()
        {
            Cancel(ref _detailCancellation);
            _view.ShowBreedLoading(NoLoadingBreedIndex);
        }

        private async Task LoadListAsync(CancellationToken cancellation)
        {
            _breedList = Array.Empty<DogBreed>();
            _view.ShowBreeds(Array.Empty<string>());
            _view.ShowBreedsStatus("Загрузка пород…", true);

            try
            {
                var breeds = await _breeds.GetBreedsAsync(FirstPageNumber, PageSize, _timeoutSeconds, cancellation);

                if (cancellation.IsCancellationRequested)
                    return;

                _breedList = breeds;
                var names = new string[breeds.Count];
                var ids = new string[breeds.Count];

                for (var i = 0; i < breeds.Count; i++)
                {
                    names[i] = breeds[i].Name;
                    ids[i] = breeds[i].Id;
                }

                _view.ShowBreeds(names, ids);
                _view.ShowBreedsStatus(breeds.Count == 0 ? "Список пород пуст." : string.Empty, false);
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
            {
                // Отменённый запрос больше не должен обновлять представление.
            }
            catch (Exception exception)
            {
                if (cancellation.IsCancellationRequested is false)
                {
                    _view.ShowBreedsStatus("Не удалось загрузить породы. Откройте вкладку повторно.", false);
                    Debug.LogWarning(
                        $"[{nameof(BreedsTabPresenter)}::{nameof(LoadListAsync)}] Breed list request failed: {exception}",
                        _view);
                }
            }
        }

        private async Task LoadDetailsAsync(string id, CancellationToken cancellation)
        {
            try
            {
                var breed = await _breeds.GetBreedAsync(id, _timeoutSeconds, cancellation);

                if (cancellation.IsCancellationRequested)
                    return;

                _view.ShowBreedLoading(NoLoadingBreedIndex);
                _modal.ShowModal(breed.Name, breed.Description, id);
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
            {
                // Отменённый запрос больше не должен обновлять представление.
            }
            catch (Exception exception)
            {
                if (cancellation.IsCancellationRequested is false)
                {
                    _view.ShowBreedLoading(NoLoadingBreedIndex);
                    _view.ShowBreedsStatus("Не удалось загрузить описание. Выберите породу ещё раз.", false);
                    Debug.LogWarning(
                        $"[{nameof(BreedsTabPresenter)}::{nameof(LoadDetailsAsync)}] Breed details request failed: {exception}",
                        _view);
                }
            }
        }

        private static void Cancel(ref CancellationTokenSource source)
        {
            if (source is null)
                return;

            source.Cancel();
            source.Dispose();
            source = null;
        }
    }
}