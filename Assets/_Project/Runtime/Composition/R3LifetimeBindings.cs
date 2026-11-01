using System;
using R3;
using Zenject;

namespace Project.Runtime.Composition
{
    /// <summary>
    /// Регистрирует общий lifetime подписок R3 отдельно для каждого DI-контекста.
    /// </summary>
    internal static class R3LifetimeBindings
    {
        /// <summary>
        /// Связывает один CompositeDisposable текущего контейнера с его штатным освобождением Zenject.
        /// </summary>
        internal static void BindR3Lifetime(this DiContainer container)
        {
            // Явно выбираем пустой конструктор: остальные конструкторы R3 принимают подписки.
            container.Bind<CompositeDisposable>().FromMethod(_ => new()).AsSingle();
            container.Bind<IDisposable>().To<CompositeDisposable>().FromResolve();
        }
    }
}