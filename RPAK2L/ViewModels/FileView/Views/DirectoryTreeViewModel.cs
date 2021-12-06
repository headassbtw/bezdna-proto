using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;
using RPAK2L.ViewModels.FileView.Types;
using RPAK2L.ViewModels.SubMenuViewModels;
using RPAK2L.Views.SubMenus;
using Size = Avalonia.Size;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DynamicData;
using RPAK2L.Dialogs;
using RPAK2L.Dialogs.HelpDialog;
using RPAK2L.Views;
using File = System.IO.File;

namespace RPAK2L.ViewModels.FileView.Views
{
    public class DirectoryTreeViewModel : ReactiveObject
    {
        public static void ConsoleLog(InAppConsoleMsg content)
        {
            if (_instance != null && _console != null && _instance.Console != null)
            {
                _instance.Console.Insert(0,content);
            }
        }
        private static ObservableCollection<InAppConsoleMsg> _console;
        public static bool __isLoading = false;
        private static string _progleft;
        public ProgressBar _bar;
        public static string progleft
        {
            get => progleft;
            set
            {
                _instance.RaiseAndSetIfChanged(ref _progleft, value);
            }
        }
        public static string progmid;
        public static string progright;
        private static double _taskTotal;
        private static int _taskProg;
        public static string loadtext = "Loading...";

        public ObservableCollection<InAppConsoleMsg> Console
        {
            get => _console;
            set
            {
                _instance.RaiseAndSetIfChanged(ref _console, value);
            }
        }
        public static double TaskProgress { get; set; }
        private bool _helpLabels = false;
        public bool ShowHelpLabels
        {
            get => _helpLabels;
            set
            {
                this.RaiseAndSetIfChanged(ref _helpLabels, value);
            }
        }
        public static double TaskTotalItems
        {
            get => _taskTotal;
            set { _instance.RaiseAndSetIfChanged(ref _taskTotal, value); }
        }
        public Window ParentWindow;
        public ReactiveCommand<Unit, Unit> ExitCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ShowHelpLabelsCommand { get; set; }
        public ReactiveCommand<Unit, Unit> HideHelpLabelsCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ReloadHeadersCommand { get; set; }
        public ReactiveCommand<Unit,Unit> OpenExportDirCommand { get; set; }
        public ReactiveCommand<Unit,Unit> TestMultiWarnCommand { get; set; }
        public ReactiveCommand<Unit,Unit> ConsoleClearCommand { get; set; }
        public ReactiveCommand<Unit,Unit> HelpWindowCommand { get; set; }
        public ObservableCollection<FileTypes> Types { get; set; }
        public ObservableCollection<PakFileInfo> Files { get; set; }
        public ObservableCollection<PakFileInfo> VisibleFiles { get; set; }
        public ObservableCollection<string> RPakChoices { get; set; }
        
        public ObservableCollection<Control> Recents { get; set; }
        public bool ConsoleEnabled
        {
            get => _consoleEn;
            set
            {
                this.RaiseAndSetIfChanged(ref _consoleEn, value);
            }
            
        }
        private bool _consoleEn = true;

        public bool DebugMenu_IsVisible
        {
            get
            {
                #if DEBUG || EXTREME_DEBUG
                return true;
                #else
                return false;
                #endif
            }
        }
        public Models.Inf FileInfo { get; set; }
        //i need a way to keep the file properties decently big on startup, but still resizable
        private string _pakfileName = "  \n";
        private string _pakfileBytes = "  \n";
        private string _pakfileOffset = "  \n";
        //...wait, that fucking worked?
        private string _currentStarpak = "  ";

        public static DirectoryTreeViewModel _instance;
        public static string LoadText
        {
            get => loadtext;
            set
            {
                _instance.RaiseAndSetIfChanged(ref loadtext, value);
            }
        }
        public static string ProgTextLeft
        {
            get => _progleft;
            set
            {
                if(!string.IsNullOrEmpty(value)) TaskProgress = int.Parse(value);
                _instance.RaiseAndSetIfChanged(ref _progleft, value);
            }
        }
        public static string ProgTextMid
        {
            get => progmid;
            set
            {
                _instance.RaiseAndSetIfChanged(ref progmid, value);
            }
        }
        public static string ProgTextRight
        {
            get => progright;
            set
            {
                _instance.RaiseAndSetIfChanged(ref progright, value);
            }
        }
        
        public bool IsLoading
        {
            get => __isLoading;
            set
            {
                this.RaiseAndSetIfChanged(ref __isLoading, value);
            }
        }
        public string InfoName
        {
            get => _pakfileName;
            set
            {
                this.RaiseAndSetIfChanged(ref _pakfileName, value);
            }
        }

        public string InfoBytes
        {
            get => _pakfileBytes;
            set
            {
                this.RaiseAndSetIfChanged(ref _pakfileBytes, value);
            }
        }

        public string InfoOffset
        {
            get => _pakfileOffset;
            set
            {
                this.RaiseAndSetIfChanged(ref _pakfileOffset, value);
            }
        }

        public Size InfAreaDesiredSize
        {
            get => new Size(0, 200);
        }
        
        
        public string CurrentStarpak
        {
            get => _currentStarpak;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentStarpak, value);
            }
        }


        private int Counter { get; set; }
        private ObservableCollection<PakFileInfo> _pakFiles;

        public void ResetTask()
        {
            _bar.IsIndeterminate = true;
            _bar.Value = 0;
            ProgTextMid = "Working...";
            ProgTextLeft = "";
            ProgTextRight = "";
        }
        
        public void SetPakFiles(ObservableCollection<PakFileInfo> pakFiles)
        {
            
            Counter = 0;
            _pakFiles = pakFiles;
        }
        public void AddPakFiles(int count = 20)
        {
            if(count > _pakFiles.Count)
                count = _pakFiles.Count;
            for (int i = 0; i < count; i++)
            {
                AddPakFile(_pakFiles[Counter]);
                Counter++;
            }
            MakeAllVisible();
        }

        private void AddPakFile(PakFileInfo info)
        {
            Files.Add(info);
        }
        private string _searchFilter = "";

        private void MakeAllVisible()
        {
            VisibleFiles.Clear();
            foreach(var pakfile in Files)
                VisibleFiles.Add(pakfile);
        }
        
        public string SearchBoxFilter
        {
            get => _searchFilter;
            set
            {
                this.RaiseAndSetIfChanged(ref _searchFilter, value);
                var filtered = Files.Where(x => x.Name.ToLower().Contains(value.ToLower())).ToList();
                Logger.Log.Info($"{filtered.Count} Items match search parameters");
                VisibleFiles.Clear();
                foreach (var whythefuckcantobservablesjustactivatewhenineedthemto in filtered)
                {
                    VisibleFiles.Add(whythefuckcantobservablesjustactivatewhenineedthemto);
                }
            }
        }
        
        
        public void OpenSettingsMenu()
        {
            var sm = new SettingsMenu();
            
            sm.ShowDialog(ParentWindow);
            sm.DataContext = new SettingsMenuViewModel(sm);
        }
        public void OpenAboutMenu()
        {
            var sm = new AboutMenu();
            sm.DataContext = new AboutMenuViewModel();
            sm.ShowDialog(ParentWindow);
        }

        public Bitmap Waifu { get; set; }

        public void LoadWaifu()
        {
            string path = Path.Combine(Environment.CurrentDirectory,
                "UserContent", "Backround.png");
            Logger.Log.Debug($"loading bg image from {path}");
            try
            {
                Bitmap f = new Bitmap(path);
                Waifu = f;
            }
            catch (Exception exc)
            {
                Logger.Log.Error("Background image could not be loaded");
            }
        }
        public Grid _centerGrid;
        public bool HasRecents
        {
            get => Recents.Count > 0;
        }

        public void ReloadRecents()
        {
            Recents.Clear();
            string path = Path.Combine(Environment.CurrentDirectory, "recents.txt");
            if (File.Exists(path))
            {
                Recents.Add(new MenuItem()
                {
                    Header = "Clear Recents",
                    Command = ReactiveCommand.Create(() => {File.WriteAllLines(path, new string[0]);})
                });
                Recents.Add(new Separator());
                string[] recents = File.ReadAllLines(path);
                for (int i = 0; i < recents.Length; i++)
                {
                    string recent = recents[i];
                    if (File.Exists(recent))
                    {
                        var item = new MenuItem();
                        item.Header = recent;
                        item.Click += (sender, args) =>(ParentWindow as DirectoryTree).Load(recent); 
                        Recents.Add(item);
                    }
                }
            }
            else
            {
                File.Create(path);
            }
        }
        
        
        public DirectoryTreeViewModel(Window context)
        {
            _instance = this;
            Console = new ObservableCollection<InAppConsoleMsg>();
            LoadWaifu();
            
            ExitCommand = ReactiveCommand.Create(() => {Program.AppMainWindow.Close(); });
            ShowHelpLabelsCommand = ReactiveCommand.Create(() => { ShowHelpLabels = true; });
            ConsoleClearCommand = ReactiveCommand.Create(() => { Console.Clear(); });
            HideHelpLabelsCommand = ReactiveCommand.Create(() => { ShowHelpLabels = false; });
            
            TestMultiWarnCommand = ReactiveCommand.Create(() =>
            {
                Program.AppMainWindow.WarningMultiDialog("Normally this would explain stuff, like why a dds failed to convert, but this is a test so i need to fluff it up and i don't want to write 'test' 20x", new []{"one","two","three"});
            });
            OpenExportDirCommand = ReactiveCommand.Create(() =>
            {
                Process.Start((OperatingSystem.IsWindows() ? "explorer.exe" : "xdg-open"),Settings.Get("ExportPath"));
            });
            HelpWindowCommand = ReactiveCommand.Create(() =>
            {
                var helpmen = new HelpWindow();
                helpmen.DataContext = new HelpContext();
                helpmen.ShowDialog(Program.AppMainWindow);
            });
            ReloadHeadersCommand = ReactiveCommand.Create(() => { Program.Headers.Init(); });
            Counter = 0;
            ParentWindow = context;
            Recents = new ObservableCollection<Control>(new List<Control>());
            ReloadRecents();
            Files = new ObservableCollection<PakFileInfo>();
            VisibleFiles = new ObservableCollection<PakFileInfo>();
            Types = new ObservableCollection<FileTypes>();
            RPakChoices = new ObservableCollection<string>();
        }
    }
}