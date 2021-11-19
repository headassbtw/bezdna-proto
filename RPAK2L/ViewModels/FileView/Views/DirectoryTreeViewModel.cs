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
        public static string progleft;
        public static string progmid;
        public static string progright;
        public Window ParentWindow;
        public ReactiveCommand<Unit, Unit> ExitCommand { get; set; }
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

        private string _pakfileName;
        private string _pakfileBytes;
        private string _pakfileOffset;
        private string _currentStarpak;

        public string ProgTextLeft
        {
            get => progleft;
            set
            {
                this.RaiseAndSetIfChanged(ref progleft, value);
            }
        }
        public string ProgTextMid
        {
            get => progmid;
            set
            {
                this.RaiseAndSetIfChanged(ref progmid, value);
            }
        }
        public string ProgTextRight
        {
            get => progright;
            set
            {
                this.RaiseAndSetIfChanged(ref progright, value);
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
                Console.WriteLine($"{filtered.Count} Items match search parameters");
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
            ExitCommand = ReactiveCommand.Create(() => {Program.AppMainWindow.Close(); });
            Counter = 0;
            ParentWindow = context;
            Files = new ObservableCollection<PakFileInfo>();
            VisibleFiles = new ObservableCollection<PakFileInfo>();
            Types = new ObservableCollection<FileTypes>();
            RPakChoices = new ObservableCollection<string>();
        }
    }
}