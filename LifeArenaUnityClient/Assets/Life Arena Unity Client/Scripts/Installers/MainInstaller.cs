using Avangardum.LifeArena.Shared;
using Avangardum.LifeArena.UnityClient.Interfaces;
using Avangardum.LifeArena.UnityClient.IO;
using Avangardum.LifeArena.UnityClient.Presenters;
using Avangardum.LifeArena.UnityClient.ServerCommunication;
using Avangardum.LifeArena.UnityClient.Views;
using Zenject;

namespace Avangardum.LifeArena.UnityClient.Installers
{
    public class MainInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<InputManager>().AsSingle();

            Container.Bind<IFieldView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IWindowManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IHeader>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IGameViewFacade>().To<GameViewFacade>().AsSingle();
            Container.Bind<GameViewManager>().AsSingle().NonLazy();

            Container.Bind<ILivingCellsArrayPreserializer>().FromMethod(() => new LivingCellsArrayPreserializer())
                .AsSingle(); // Normal binding causes MissingMethodException in the WebGL build
            Container.Bind<IServerFacade>().To<ServerFacade>().AsSingle();
            
            Container.Bind<FieldInputPresenter>().AsSingle().NonLazy();
            Container.Bind<GamePresenter>().AsSingle().NonLazy();
        }
    }
}