using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RPAK2L.CrimesAgainstHumanity
{
    public class WaifuView : UserControl
    {
        public WaifuView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}