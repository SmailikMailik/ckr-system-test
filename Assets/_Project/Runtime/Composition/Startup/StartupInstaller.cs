using Zenject;

namespace Project.Runtime.Composition.Startup
{
    /// <summary>
    /// Регистрирует переход из загрузочной сцены после подготовки глобальных данных пользователя.
    /// </summary>
    internal sealed class StartupInstaller : MonoInstaller
    {
        private const string MainSceneName = "Main";

        /// <summary>
        /// Выбирает штатную загрузку Main с глобальным ProjectContext в качестве родительского контейнера.
        /// </summary>
        public override void InstallBindings()
        {
            Container.Bind<IStartupSceneRouter>().To<ZenjectStartupSceneRouter>().AsSingle().WithArguments(MainSceneName);
        }
    }
}