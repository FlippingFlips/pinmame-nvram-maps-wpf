using PinMAME.NvMaps.Win.Module.ViewModels;
using PinMAME.NvMaps.Win.Module.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Unity;
using Unity.Lifetime;

namespace PinMAME.NvMaps.Win.Module
{
    public class MyModule : IModule
    {
        private readonly IRegionManager regionManager;
        private readonly IUnityContainer container;

        public MyModule(IRegionManager regionManager, IUnityContainer container)
        {
            this.regionManager = regionManager;
            this.container = container;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //register services, especially if just used in this module
            //containerRegistry.RegisterSingleton<IMyService, MyService>();

            //register views for navigating
            //containerRegistry.RegisterForNavigation<MySearchView>();

            //register dialogs for IDialogService
            //containerRegistry.RegisterDialog<RandomDialogView, RandomDialogViewModel>();

            //register singleton view models
            container.RegisterInstance(container.Resolve<HighScoresViewModel>(), new ContainerControlledLifetimeManager());
            container.RegisterInstance(container.Resolve<LatestScoresViewModel>(), new ContainerControlledLifetimeManager());
            container.RegisterInstance(container.Resolve<MappingViewModel>(), new ContainerControlledLifetimeManager());
            container.RegisterInstance(container.Resolve<ModeChampionsViewModel>(), new ContainerControlledLifetimeManager());
            container.RegisterInstance(container.Resolve<AdjustmentsViewModel>(), new ContainerControlledLifetimeManager());
            container.RegisterInstance(container.Resolve<AuditsViewModel>(), new ContainerControlledLifetimeManager());

            //register views with regions
            regionManager.RegisterViewWithRegion("LatestScoresRegion", typeof(LatestScores));
            regionManager.RegisterViewWithRegion("HighScoresRegion", typeof(HighScores));
            regionManager.RegisterViewWithRegion("MappingRegion", typeof(Mapping));
            regionManager.RegisterViewWithRegion("ModeChampionsRegion", typeof(ModeChampions));
            regionManager.RegisterViewWithRegion("AdjustmentsRegion", typeof(Adjustments));
            regionManager.RegisterViewWithRegion("AuditsRegion", typeof(Audits));
        }
    }
}