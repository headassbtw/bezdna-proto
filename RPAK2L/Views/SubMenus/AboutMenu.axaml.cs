using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace RPAK2L.Views.SubMenus
{
    public class AboutMenu : Window
    {
        public AboutMenu()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void GithubButton_OnClick(object? sender, RoutedEventArgs e)
        {
            
        }
    }
}