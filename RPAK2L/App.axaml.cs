using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RPAK2L.ViewModels;
using RPAK2L.ViewModels.FileView.Views;
using RPAK2L.Views;

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
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new DirectoryTree()
                {
                    DataContext = new DirectoryTreeViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}