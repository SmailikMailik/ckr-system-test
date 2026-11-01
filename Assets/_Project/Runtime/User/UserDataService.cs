using System;

namespace Project.User
{
    /// <summary>
    /// Загружает пользовательскую сессию и сохраняет завершённые изменения через выбранное хранилище.
    /// </summary>
    public sealed class UserDataService : IDisposable
    {
        private readonly UserSession _session;
        private readonly UserConfig _config;
        private readonly IUserDataStore _store;
        private bool _initialized;
        private bool _dirty;

        /// <summary>
        /// Сохраняет сессию, настройки первого запуска и хранилище для последующей загрузки в Initialize.
        /// </summary>
        public UserDataService(UserSession session, UserConfig config, IUserDataStore store)
        {
            _session = session;
            _config = config;
            _store = store;
        }

        /// <summary>
        /// Загружает данные при первом успешном вызове; ошибка чтения передаётся вызывающему коду и допускает
        /// повтор загрузки.
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
                return;

            var saved = _store.Load();
            _session.Initialize(_config, saved);
            _session.Changed += MarkDirty;
            _dirty = saved is null || saved.Energy != _session.Energy.Current;
            _initialized = true;
        }

        /// <summary>
        /// Сохраняет только изменённое состояние; при ошибке оставляет его ожидающим следующей попытки.
        /// </summary>
        public void Save()
        {
            if (_initialized is false || _dirty is false)
                return;

            var snapshot = _session.Snapshot;
            _store.Save(snapshot);
            var current = _session.Snapshot;
            _dirty = current.Currency != snapshot.Currency || current.Energy != snapshot.Energy;
        }

        /// <summary>
        /// Отключает наблюдение; финальное сохранение выполняет владелец lifecycle до освобождения сервиса.
        /// </summary>
        public void Dispose()
        {
            if (_initialized)
                _session.Changed -= MarkDirty;
        }

        private void MarkDirty(UserSnapshot snapshot)
        {
            _dirty = true;
        }
    }
}