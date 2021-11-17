using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RPAK2L.Views.SubMenus
{
    public class SettingsMenu : Window
    {
        public SettingsMenu()
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
    }
}