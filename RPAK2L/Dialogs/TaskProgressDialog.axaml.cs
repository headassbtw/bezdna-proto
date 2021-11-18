using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RPAK2L.Dialogs
{
    public class TaskProgressDialog : Window
    {
        public TaskProgressDialog()
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

        private void ProgressBar_Init(object? sender, EventArgs e)
        {
            var progbar = sender as ProgressBar;
            
        }
    }
}