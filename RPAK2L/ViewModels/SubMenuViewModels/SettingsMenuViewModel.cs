using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;
using RPAK2L.Backend;
using RPAK2L.Dialogs;

namespace RPAK2L.ViewModels.SubMenuViewModels
{
    public class SettingsMenuViewModel : ViewModelBase
    {
        public EventHandler ApplyFired { get; set; }
        public ReactiveCommand<Unit, Unit> ApplyCommand { get; set; }
        public string WindowTitle => "Settings";
        public static string _gamePath = "";
        public static string _exportPath;
        public static bool _experimentalFeatures = false;
        public static bool _onlyExportHighestRes = true;
        public string GamePath
        {
            get => _gamePath;
            set
            {
                Logger.Log.Info("Game Directory updated: " + value);
                this.RaiseAndSetIfChanged(ref _gamePath, value);
            }
        }

        public string ExportPath
        {
            get => _exportPath;
            set
            {
                Logger.Log.Info("Export Directory updated: " + value);
                this.RaiseAndSetIfChanged(ref _exportPath, value);
            }
        }

        public bool ExperimentalFeatures
        {
            get => _experimentalFeatures;
            set
            {
                this.RaiseAndSetIfChanged(ref _experimentalFeatures, value);
            }
        }

        public bool OnlyExportHighestRes
        {
            get => _onlyExportHighestRes;
            set
            {
                this.RaiseAndSetIfChanged(ref _onlyExportHighestRes, value);
            }
        }

        public ObservableCollection<Grid> SettingsItems { get; set; }

        public void BindFolderSetting(string binding, string title, string iniKey, string[]? mustContainSubDir = null)
        {
            var btn = StringSetting(binding, title, iniKey);
            btn.Item1.Click += async (sender, args) =>
            {
                Window.ShowDirDialogSync(title,btn.Item2, mustContainSubDir);
            };
        }

        public void BindBoolSetting(string binding, string title, string iniKey)
        {
            var btn = BoolSetting(binding, title, iniKey);
        }
        
        CheckBox BoolSetting(string binding, string title, string iniKey)
        {
            var boxBinding = new Binding(binding);
            var g = new Grid();
            g.MinWidth = 680;
            g.MaxWidth = 7274; //HOW THE FUCK DO I MAKE THIS BITCH EXPAND
            g.RowDefinitions = RowDefinitions.Parse("Auto,Auto");
            g.ColumnDefinitions.Add(new ColumnDefinition(100, GridUnitType.Star));
            TextBlock header = new TextBlock();
            header.Text = title;
            Grid.SetRow(header,0);
            g.Children.Add(header);
            Grid pathGrid = new Grid();
            pathGrid.ColumnDefinitions.Add(new ColumnDefinition(100, GridUnitType.Star));
            Grid.SetRow(pathGrid,1);
            if (Settings.Get(iniKey, "null") == "null")
            {
                Settings.Set(iniKey,"false");
            }
            CheckBox pathBox = new CheckBox()
            {
                IsChecked = bool.Parse(Settings.Get(iniKey))
            };
            pathBox.Bind(CheckBox.IsCheckedProperty, boxBinding);
            var text = pathBox.GetObservable(CheckBox.IsCheckedProperty);
            bool val = false;
            text.Subscribe(value =>
            {
                Logger.Log.Info($"Binding \"{binding}\" changed in box to {value}");
                val = value.HasValue ? value.Value : false;
            });
            ApplyFired += (sender, args) =>
            {
                Settings.Set(iniKey, val.ToString());
            };
            pathBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            Grid.SetColumn(pathBox,0);
            pathGrid.Children.Add(pathBox);
            g.Children.Add(pathGrid);
            SettingsItems.Add(g);
            pathBox.Initialized += (sender, args) =>
            {
                pathBox.IsChecked = bool.Parse(Settings.Get(iniKey));
            };
            return pathBox;
        }
        (Button,TextBox) StringSetting(string binding, string title, string iniKey)
        {
            var boxBinding = new Binding(binding);
            var g = new Grid();
            g.MinWidth = 680;
            g.MaxWidth = 7274; //HOW THE FUCK DO I MAKE THIS BITCH EXPAND
            g.RowDefinitions = RowDefinitions.Parse("Auto,Auto");
            g.ColumnDefinitions.Add(new ColumnDefinition(100, GridUnitType.Star));
            TextBlock header = new TextBlock();
            header.Text = title;
            Grid.SetRow(header,0);
            g.Children.Add(header);
            Grid pathGrid = new Grid();
            pathGrid.ColumnDefinitions.Add(new ColumnDefinition(100, GridUnitType.Star));
            pathGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            Grid.SetRow(pathGrid,1);
            if (Settings.Get(iniKey,"null") == "null")
            {
                Settings.Set(iniKey,"/tmp");
            }
            TextBox pathBox = new TextBox()
            {
                IsEnabled = false,
                Text = Settings.Get(iniKey),
                [!TextBox.TextProperty] = boxBinding
            };
            pathBox.Bind(TextBox.TextProperty, boxBinding);
            var text = pathBox.GetObservable(TextBox.TextProperty);
            string val = "";
            text.Subscribe(value =>
            {
                if(String.IsNullOrEmpty(value)) return;
                Logger.Log.Info($"Binding \"{binding}\" changed in box to {value}");
                val = value;
            });
            ApplyFired += (sender, args) =>
            {
                Settings.Set(iniKey, val);
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
            pathBox.Initialized += (sender, args) => { pathBox.Text = Settings.Get(iniKey);};
            return (browseButton, pathBox);
        }

        void Apply()
        {
            ApplyFired.Invoke(this, EventArgs.Empty);
            Settings.Save();
            Logger.Log.Info("Preferences saved to ini file");
            Program.AppMainWindow.WarningDialog("Settings applied, a restart of the program may be required for some items to take effect");
        }
        
        public Window Window;
        public SettingsMenuViewModel(Window targetWindow)
        {
            Settings.Load();
            ApplyCommand = ReactiveCommand.Create(Apply);
            SettingsItems = new ObservableCollection<Grid>();
            Window = targetWindow;
            BindFolderSetting("GamePath", "Game Directory", "GamePath",new [] {"r2","paks","Win64"});
            BindFolderSetting("ExportPath", "Export Directory", "ExportPath");
            BindBoolSetting("ExperimentalFeatures", "Experimental Features","Experiments");
            BindBoolSetting("OnlyExportHighestRes", "Only export highest resolution texture","OnlyHighestRes");
        }
    }
}
