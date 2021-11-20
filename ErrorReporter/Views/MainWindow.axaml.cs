using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Document;

namespace ErrorReporter.Views
{
    public partial class MainWindow : Window
    {
        private readonly TextEditor _textEditor;
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            /*
            _textEditor = this.FindControl<TextEditor>("RichLogBox");
            _textEditor.Background = Brushes.Transparent;
            _textEditor.Foreground = Brushes.Cyan;
            _textEditor.ShowLineNumbers = true;
            _textEditor.Document = new TextDocument();
            _textEditor.Document.Insert(0,"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAa\n\n\n\nAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            _textEditor.Document.RunUpdate();
            */
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}