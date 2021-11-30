using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace RPAK2L.Dialogs
{
    public static class WarningMultiItemDialogExtensions
    {
        public static void WarningMultiDialog(this Window parent, string warning, string[] content, string imae = "Icon_full")
        {
            var diag = new WarningMultiItemDialog();
            diag.DataContext = new WarningMultiItemDataContext();
            var contx = diag.DataContext as WarningMultiItemDataContext;
            contx.Warning = warning;
            contx.Content = content;
            diag.ShowDialog(parent);
        }
    }
    public class WarningMultiItemDialog : Window
    {
        public WarningMultiItemDialog()
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
        private void Okay_OnClick(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}