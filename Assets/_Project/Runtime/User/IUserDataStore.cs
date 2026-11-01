namespace Project.User
{
    /// <summary>
    /// Сохраняет и загружает целый снимок пользователя, скрывая формат и место хранения.
    /// </summary>
    public interface IUserDataStore
    {
        /// <summary>
        /// Возвращает сохранённый снимок; null означает только отсутствие сохранения.
        /// </summary>
        UserSnapshot Load();

        /// <summary>
        /// Сохраняет завершённый снимок; при ошибке предыдущее сохранение должно оставаться доступным.
        /// </summary>
        void Save(UserSnapshot snapshot);
    }
}