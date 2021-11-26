using System;
using System.IO;
using System.Linq;
using bezdna_proto.Titanfall2.FileTypes;
using RPAK2L.Dialogs;
using RPAK2L.ViewModels.FileView.Types;
using RPAK2L.ViewModels.SubMenuViewModels;
using File = System.IO.File;

namespace RPAK2L.Tools
{
    public class Exporters
    {
        public static void TextureData(PakFileInfo file, string LastSelectedDirectory, string exportDir, string exportSub = "", bool ExportStatic = true, bool material = false)
        {
            Logger.Log.Info($"Exporting Texture to {exportDir}");
            if(file == null) return;
            var tex = file.SpecificTypeFile as Texture;
            //FUCK YOU WE CAN ACCESS STATIC STUFF NOW BITCHHHH
            /*
            if (tex.TextureDatas.Where(t => t.streaming).ToList().Count <= 0)
            {
                if(SettingsMenuViewModel._experimentalFeatures)
                    Program.AppMainWindow.WarningDialog("This is an experimental feature, use at your own risk");
                else
                {
                    Program.AppMainWindow.WarningDialog("Unable to access this texture (not implemented)");
                    return;
                }
            }*/

            string pak = file.Pak.StarPaks[0]
                .Substring(file.Pak.StarPaks[0].LastIndexOf('\\')+1);
            
            var compression = tex.Algorithm.ToUpper();
            Logger.Log.Debug($"{compression} | 0x{tex.BaseFile.StarpakOffset.ToString("X").ToUpper()}");
            Logger.Log.Debug(exportDir);
            string ex = Path.Combine(exportDir, exportSub,tex.Name).Replace('\\',Path.DirectorySeparatorChar).Replace('/',Path.DirectorySeparatorChar);
            Logger.Log.Debug(ex);
            int type = ex.LastIndexOf('_') + 1;
            string textype = "";
            if (material)
            {
                textype = ex.Substring(type);
                ex = ex.Substring(0,type);
            }
            
            
            
            foreach (var text in (material || SettingsMenuViewModel._onlyExportHighestRes) ? tex.TextureDatas.Take(1) : tex.TextureDatas)
            {
                
                if(compression == "DXT1" || compression.StartsWith("BC"))
                {
                    Logger.Log.Debug($"ExportingMipMap ({text.width}x{text.height})");
                    byte[] buf = new byte[text.size];
                    string filename = Path.Combine(ex, (material ? textype : text.height) + ".dds");
                    if (SettingsMenuViewModel._onlyExportHighestRes)
                    {
                        filename = ex + '_'+ (material ? textype : text.height) + ".dds";
                        Directory.CreateDirectory(ex.Substring(0,ex.LastIndexOf(Path.DirectorySeparatorChar)));
                    }
                    else
                    {
                        Directory.CreateDirectory(ex);
                    }
                    var fs = File.Create(filename);
                    if (text.streaming)
                    {
                        Logger.Log.Info("Opening starpak stream [STREAMING]");
                        FileStream spr = new FileStream(
                            Path.Combine(LastSelectedDirectory, "r2", "paks", "Win64", pak),
                            FileMode.Open);
                        
                        spr.Seek(text.seek, SeekOrigin.Begin);
                        spr.Read(buf);
                    }
                    else
                    {
                        Logger.Log.Info("Opening rpak stream [STATIC]");
                        file.Pak.reader.BaseStream.Seek(text.seek, SeekOrigin.Begin);
                        file.Pak.reader.Read(buf);
                    }
                    fs.Write(Program.Headers.GetCustomRes((uint)text.width, (uint)text.height, compression));
                    fs.Write(buf);
                    fs.Close();
                }
                else
                {
                    Program.AppMainWindow.WarningDialog("Unsupported Compression Algoritm");
                }
            }
        }
    }
}