﻿using System.Windows;
using Arkivverket.Arkade.UI.Views;
using Arkivverket.Arkade.Util;
using Autofac;
using Prism.Autofac;
using Prism.Modularity;

namespace Arkivverket.Arkade.UI
{
    public class Bootstrapper : AutofacBootstrapper
    {
        protected override void ConfigureModuleCatalog()
        {
            var catalog = (ModuleCatalog) ModuleCatalog;
            catalog.AddModule(typeof(ModuleAModule));
        }

        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            base.ConfigureContainerBuilder(builder);
            builder.RegisterModule(new ArkadeAutofacModule());

            builder.RegisterTypeForNavigation<TestRunner>();
            builder.RegisterTypeForNavigation<CreatePackage>();
            builder.RegisterTypeForNavigation<LoadArchiveExtraction>();
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }
    }
}