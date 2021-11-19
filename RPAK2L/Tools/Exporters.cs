using System;
using System.IO;
using System.Linq;
using bezdna_proto.Titanfall2.FileTypes;
using RPAK2L.Dialogs;
using RPAK2L.ViewModels.FileView.Types;
using File = System.IO.File;

namespace RPAK2L.Tools
{
    public class Exporters
    {
        public static void TextureData(PakFileInfo file, string LastSelectedDirectory, string exportSub = "", bool ExportStatic = true, bool material = false)
        {
            Logger.Log.Info("Exporting Texture...");
            if(file == null) return;
            var tex = file.SpecificTypeFile as Texture;
            if (tex.TextureDatas.Where(t => t.streaming).ToList().Count <= 0)
            {
                #if DEBUG || EXTREME_DEBUG
                if(ExportStatic)
                    Program.AppMainWindow.WarningDialog("This feature does not officially work, but i've enabled it in debug builds, for testing");
                #elif Release
                        this.WarningDialog("Unable to access this texture (not implemented)");
                        break;
                #endif
            }

            string pak = file.Pak.StarPaks[0]
                .Substring(file.Pak.StarPaks[0].LastIndexOf('\\')+1);
                    
            Console.WriteLine(tex.StarpakNum);
            var compression = tex.Algorithm.ToUpper();
            Console.WriteLine(compression);
            Console.WriteLine(tex.BaseFile.StarpakOffset);
            string ex = Path.Combine(Environment.CurrentDirectory, "Export", exportSub,tex.Name).Replace('\\',Path.DirectorySeparatorChar).Replace('/',Path.DirectorySeparatorChar);
            int type = ex.LastIndexOf('_') + 1;
            string textype = "";
            if (material)
            {
                textype = ex.Substring(type);
                ex = ex.Substring(0,type);
            }
            
            Directory.CreateDirectory(ex);
            foreach (var text in material ? tex.TextureDatas.Take(1) : tex.TextureDatas)
            {
                if (text.streaming || ExportStatic)
                {
                    if(compression == "DXT1" || compression.StartsWith("BC"))
                    {
                        Logger.Log.Debug($"ExportingMipMap ({text.width}x{text.height})");
                        byte[] buf = new byte[text.size];
                        var fs = File.Create(Path.Combine(ex, (material ? textype : text.height) + ".dds"));
                        Logger.Log.Debug("Opening starpak stream");
                        FileStream spr = new FileStream(
                            Path.Combine(LastSelectedDirectory, "r2", "paks", "Win64", pak),
                            FileMode.Open);

                        spr.Seek(text.seek, SeekOrigin.Begin);
                        spr.Read(buf);
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
}