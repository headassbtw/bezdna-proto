using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace RPAK2L.ViewModels.SubMenuViewModels
{
    public class SettingsMenuViewModel : ViewModelBase
    {
        public string WindowTitle => "Settings";

        public ObservableCollection<Grid> SettingsItems { get; }

        public SettingsMenuViewModel()
        {
            SettingsItems = new ObservableCollection<Grid>();
            for(int i = 0; i < 4; i++)
            {
                var g = new Grid();
                g.MinWidth = 680;
                g.MaxWidth = 7274; //HOW THE FUCK DO I MAKE THIS BITCH EXPAND
                g.RowDefinitions = RowDefinitions.Parse("Auto,Auto");
                g.ColumnDefinitions.Add(new ColumnDefinition(100, GridUnitType.Star));
                g.ShowGridLines = true;
                TextBlock header = new TextBlock();
                header.Text = $"Setting {i}";
                Grid.SetRow(header,0);
                g.Children.Add(header);

                Grid pathGrid = new Grid();
                pathGrid.ColumnDefinitions.Add(new ColumnDefinition(100, GridUnitType.Star));
                pathGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                Grid.SetRow(pathGrid,1);
                TextBox pathBox = new TextBox()
                {
                    IsEnabled = false
                };
                pathBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                Button browseButton = new Button()
                {
                    Content = "Browse"
                };
                Grid.SetColumn(pathBox,0);
                pathGrid.Children.Add(pathBox);
                Grid.SetColumn(browseButton,1);
                pathGrid.Children.Add(browseButton);
                g.Children.Add(pathGrid);
                SettingsItems.Add(g);
            }
        }
    }
}