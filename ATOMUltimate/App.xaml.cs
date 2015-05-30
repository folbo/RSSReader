using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml.Serialization;
using ATOMUltimate.View;
using Autofac;

namespace ATOMUltimate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigureContainer();
            ComposeObjects();
            Current.MainWindow.Show();
        }

        private void ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly());



            var container = builder.Build();



            IoC.Initialize(container);
        }

        private void ComposeObjects()
        {
            Current.MainWindow = IoC.Resolve<MainWindow>();


            feedType feed = new feedType();

            
        }
    }
}