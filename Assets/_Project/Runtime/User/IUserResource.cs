namespace Project.User
{
    /// <summary>
    /// Предоставляет сессии проверку и применение изменения; сессия публикует итоговый снимок всей
    /// операции.
    /// </summary>
    internal interface IUserResource
    {
        /// <summary>
        /// Проверяет изменение по собственным правилам ресурса.
        /// </summary>
        bool CanApply(int delta);

        /// <summary>
        /// Применяет проверенное изменение; уведомление о завершении операции публикует сессия.
        /// </summary>
        void Apply(int delta);
    }
}