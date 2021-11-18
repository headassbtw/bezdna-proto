using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        public Window ParentWindow;
        public ObservableCollection<FileTypes> Types { get; set; }
        public ObservableCollection<PakFileInfo> Files { get; }
        public ObservableCollection<string> RPakChoices { get; }

        public Models.Inf FileInfo { get; set; }
        public string InfoName
        {
            get => FileInfo.Name;
            set
            {
                var fileInfoName = FileInfo.Name;
                this.RaiseAndSetIfChanged(ref fileInfoName, value);
            }
        }

        public string InfoBytes
        {
            get => FileInfo.Size;
            set
            {
                var fileInfoSize = FileInfo.Size;
                this.RaiseAndSetIfChanged(ref fileInfoSize, value);
            }
        }

        public string InfoOffset
        {
            get => FileInfo.Offset;
            set
            {
                var fileInfoOffset = FileInfo.Offset;
                this.RaiseAndSetIfChanged(ref fileInfoOffset, value);
            }
        }

        private int Counter { get; set; }
        private ObservableCollection<PakFileInfo> _pakFiles;

        public void SetPakFiles(ObservableCollection<PakFileInfo> pakFiles)
        {
            _pakFiles = pakFiles;
        }
        public void AddPakFiles(int count = 20)
        {
            for (int i = 0; i < count; i++)
            {
                AddPakFile(_pakFiles[i]);
            }
        }

        private void AddPakFile(PakFileInfo info)
        {
            Files.Add(info);
        }
        
        
        public void OpenSettingsMenu()
        {
            var sm = new SettingsMenu();
            sm.DataContext = new SettingsMenuViewModel();
            sm.ShowDialog(ParentWindow);
        }
        public void OpenAboutMenu()
        {
            var sm = new AboutMenu();
            sm.DataContext = new AboutMenuViewModel();
            sm.ShowDialog(ParentWindow);
        }
        
        
        
        
        
        
        public DirectoryTreeViewModel(Window context)
        {
            ParentWindow = context;
            Files = new ObservableCollection<PakFileInfo>();
            Types = new ObservableCollection<FileTypes>();
            RPakChoices = new ObservableCollection<string>();
        }
    }
}