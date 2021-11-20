using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using RPAK2L.ViewModels.FileView.Types;
using RPAK2L.ViewModels.SubMenuViewModels;
using RPAK2L.Views;
using RPAK2L.Views.SubMenus;
using File = RPAK2L.ViewModels.FileView.Types.File;

namespace RPAK2L.ViewModels.FileView.Views
{
    public class DirectoryTreeViewModel : ReactiveObject
    {
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
        public ObservableCollection<FileTypes> Types { get; set; }
        public ObservableCollection<PakFileInfo> Files { get; set; }
        public ObservableCollection<PakFileInfo> VisibleFiles { get; set; }
        public ObservableCollection<string> RPakChoices { get; set; }

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
                Console.WriteLine(value);
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
        
        
        
        
        
        
        public DirectoryTreeViewModel(Window context)
        {
            
            _instance = this;
            ExitCommand = ReactiveCommand.Create(() => {Program.AppMainWindow.Close(); });
            ShowHelpLabelsCommand = ReactiveCommand.Create(() => { ShowHelpLabels = true; });
            HideHelpLabelsCommand = ReactiveCommand.Create(() => { ShowHelpLabels = false; });
            Counter = 0;
            ParentWindow = context;
            Files = new ObservableCollection<PakFileInfo>();
            VisibleFiles = new ObservableCollection<PakFileInfo>();
            Types = new ObservableCollection<FileTypes>();
            RPakChoices = new ObservableCollection<string>();
        }
    }
}