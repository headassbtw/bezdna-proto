using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;

namespace RPAK2L.Styles
{
    public class HelpItem : TemplatedControl
    {
        public static readonly StyledProperty<string> LabelProperty =
            AvaloniaProperty.Register<HelpItem, string>(nameof(Help_Text));
        public string Help_Text
        {
            get { return GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public HelpItem()
        {
            this.InitializeComponent();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}