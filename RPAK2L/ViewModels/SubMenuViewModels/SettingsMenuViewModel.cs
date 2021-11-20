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

namespace RPAK2L.ViewModels.SubMenuViewModels
{
    public class SettingsMenuViewModel : ViewModelBase
    {
        private Ini _settings;
        public EventHandler ApplyFired { get; set; }
        public ReactiveCommand<Unit, Unit> ApplyCommand { get; set; }
        public string WindowTitle => "Settings";
        private string _gamePath = "";
        private string _exportPath;
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


        public ObservableCollection<Grid> SettingsItems { get; set; }

        public void BindFolderSetting(string binding, string title, string iniKey, string[]? mustContainSubDir = null)
        {
            var btn = StringSetting(binding, title, iniKey);
            btn.Item1.Click += async (sender, args) =>
            {
                Window.ShowDirDialogSync(title,btn.Item2, mustContainSubDir);
            };
        }

        CheckBox BoolSetting(bool binding, string title, string iniKey)
        {
            //TODO: bool settings
            Grid g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition(100, GridUnitType.Star));
            g.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            g.MinWidth = 680;
            g.MaxWidth = 7274;
            var checkBox = new CheckBox
            {
                Content = title,
                IsChecked = binding
            };
            return checkBox;
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
            if (_settings.GetValue(iniKey, "", "null") == "null")
            {
                _settings.WriteValue(iniKey,"/tmp");
                _settings.Save();
            }
            TextBox pathBox = new TextBox()
            {
                IsEnabled = false,
                Text = _settings.GetValue(iniKey,"","null"),
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
                _settings.WriteValue(iniKey, val);
                
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
            pathBox.Initialized += (sender, args) => { pathBox.Text = _settings.GetValue(iniKey, "", "null");};
            return (browseButton, pathBox);
        }

        void Apply()
        {
            _settings.Save();
            Logger.Log.Info("Preferences saved to ini file");
            ApplyFired.Invoke(this, EventArgs.Empty);
        }
        
        public Window Window;
        public SettingsMenuViewModel(Window targetWindow)
        {
            _settings = new Ini(Path.Combine(Environment.CurrentDirectory, "settings.ini"));
            _settings.Load();
            ApplyCommand = ReactiveCommand.Create(Apply);
            SettingsItems = new ObservableCollection<Grid>();
            Window = targetWindow;
            BindFolderSetting("GamePath", "Game Directory", "GamePath",new [] {"r2","paks","Win64"});
            BindFolderSetting("ExportPath", "Export Directory", "ExportPath");
        }
    }
}
