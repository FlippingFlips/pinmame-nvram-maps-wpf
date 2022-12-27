using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System.Windows;
using PinMAME.NvMaps.Win.Shell.Views;
using MahApps.Metro.Controls.Dialogs;

namespace PinMAME.NvMaps.Win.Shell
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell() => Container.Resolve<MainWindow>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {        
            containerRegistry.RegisterSingleton<IDialogCoordinator, DialogCoordinator>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);

            moduleCatalog.AddModule<Module.MyModule>();
        }

        private void PrismApplication_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"{e.Exception.Message} - {e.Exception.InnerException?.Message}");
            e.Handled = false;
        }
    }
}
