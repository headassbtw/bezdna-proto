using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RPAK2L.Program.ViewModels;
using RPAK2L.Program.ViewModels.FileView.Views;
using RPAK2L.Program.Views;

namespace RPAK2L
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
        public override void OnFrameworkInitializationCompleted()
        {

            try
            {
                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.MainWindow = new DirectoryTree();
                    desktop.MainWindow.DataContext = new DirectoryTreeViewModel(desktop.MainWindow);
                    Program.Program.AppMainWindow = desktop.MainWindow;
                }

                base.OnFrameworkInitializationCompleted();
            }
            catch (Exception e)
            {
                Console.WriteLine("ayo!?");
            }
        }
    }
}