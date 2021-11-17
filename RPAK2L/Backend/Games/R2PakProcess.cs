using System.Collections.ObjectModel;
using System.IO;
using bezdna_proto;
using bezdna_proto.Titanfall2;
using bezdna_proto.Titanfall2.FileTypes;
using DynamicData;
using RPAK2L.ViewModels.FileView.Types;

namespace RPAK2L.Backend.Games
{
    public class R2Pak
    {
        public bezdna_proto.Titanfall2.RPakFile Pak;
        public ObservableCollection<PakFileInfo> PakInfos;
        public ObservableCollection<FileEntryInternal> Files;
        public R2Pak(FileStream PakStream)
        {
            PakStream.Position = 0;
            Pak = new RPakFile(PakStream);

            PakInfos = new ObservableCollection<PakFileInfo>();
            
            foreach (FileEntryInternal file in Pak.FilesInternal)
            {
                string extension = file.ShortName;
                
                object y = null;

                switch (extension)
                {
                    case "txtr":
                        var t = new Texture(Pak, file);
                        if (t.Name.Contains("col"))
                        PakInfos.Add(new PakFileInfo(file)
                        {
                            Pak = Pak,
                            SpecificTypeFile = t,
                            Name = t.Name
                        });
                        break;
                    case "matl":
                        var m = new Material(Pak, file);
                        PakInfos.Add(new PakFileInfo(file)
                        {
                            Pak = Pak,
                            SpecificTypeFile = m,
                            Name = m.Name
                        });
                        break;
                    case "shdr":
                        var s = new Shader(Pak, file);
                        PakInfos.Add(new PakFileInfo(file)
                        {
                            Pak = Pak,
                            SpecificTypeFile = s,
                            Name = s.Name
                        });
                        break;
                    case "dtbl":
                        var d = new DataTables(Pak, file);
                        PakInfos.Add(new PakFileInfo(file)
                        {
                            Pak = Pak,
                            SpecificTypeFile = d,
                            Name = file.GUID.ToString("X").PadLeft(16, '0')
                        });
                        break;
                    default:
                        PakInfos.Add(new PakFileInfo(file)
                        {
                            Name = file.GUID.ToString("X").PadLeft(16, '0') + file.ShortName,
                            Pak = Pak
                        });
                        break;
                }
                
                
                
            }
            
        }
    }
}