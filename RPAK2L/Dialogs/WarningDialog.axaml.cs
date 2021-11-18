using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace RPAK2L.Dialogs
{
    public static class WarningDialogExtensions
    {
        public static void WarningDialog(this Window parent, string content, string imae = "Icon_full")
        {
            var diag = new WarningDialog();
            diag.DataContext = new WarningDataContext();
            var contx = diag.DataContext as WarningDataContext;
            contx.Content = content;
            diag.ShowDialog(parent);
        }
    }
    public class WarningDialog : Window
    {
        public WarningDialog()
        {
            this.DataContext = new WarningDataContext();
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Okay_OnClick(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}