using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RPAK2L.Dialogs;

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

        //function that opens the project's GitHub page
        private void OpenProjectGitHub()
        {
            if (OperatingSystem.IsWindows())
            {
                Process.Start("https://github.com/headassbtw/RPAK2L");
            }
            else if (OperatingSystem.IsMacOS())
            {
                Process.Start("open", "https://github.com/headassbtw/RPAK2L");
            }
            else if (OperatingSystem.IsLinux())
            {
                Process.Start("xdg-open", "https://github.com/headassbtw/RPAK2L");
            }
            
        }

        private void ShitOnXbox(object? sender, RoutedEventArgs e)
        {
            Program.AppMainWindow.WarningDialog("CONSIDER YOUR XBOX SHAT ON NERDDDDD");
            
            
        }
        
        private void GithubButton_OnClick(object? sender, RoutedEventArgs e)
        {
            OpenProjectGitHub();
        }
    }
}