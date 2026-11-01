namespace Project.Runtime.Composition.Startup
{
    /// <summary>
    /// Задаёт переход из Startup в главную сцену приложения.
    /// </summary>
    internal interface IStartupSceneRouter
    {
        /// <summary>
        /// Начинает асинхронное открытие главной сцены вместо Startup.
        /// </summary>
        void OpenMain();
    }
}