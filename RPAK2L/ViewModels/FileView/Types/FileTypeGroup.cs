using System.Collections.ObjectModel;
using bezdna_proto.Titanfall2;
using RPAK2L.Backend;

namespace RPAK2L.ViewModels.FileView.Types
{
    public class FileTypes
    {
        public ObservableCollection<PakFileInfo> Files { get; set; }
        public string Name { get; set; }
        public FileTypes()
        {
            Files = new ObservableCollection<PakFileInfo>();
            Name = "ASS2";
        }
    }

    public class PakFileInfo
    {
        public string Name { get; set; }
        public RPakFile Pak { get; set; }
        public FileType Type { get; set; }
        public FileEntryInternal File { get; set; }
        public object SpecificTypeFile { get; set; }


        public PakFileInfo()
        {
            
        }

        public PakFileInfo(FileEntryInternal pakFile)
        {
            File = pakFile;
            Type = pakFile.Type();
            
            Name = pakFile.GUID.ToString();
        }
        

    }
}