using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ReactiveUI;
using RPAK2L.ViewModels.FileView.Types;
using RPAK2L.Views;
using File = RPAK2L.ViewModels.FileView.Types.File;

namespace RPAK2L.ViewModels.FileView.Views
{
    public class DirectoryTreeViewModel : ReactiveObject
    {
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


        public DirectoryTreeViewModel(string path = null)
        {
            Files = new ObservableCollection<PakFileInfo>();
            Types = new ObservableCollection<FileTypes>();
            RPakChoices = new ObservableCollection<string>();
        }
    }
}