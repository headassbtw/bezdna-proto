using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Avalonia.Threading;
using BCnEncoder.Decoder.Options;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using BCnEncoder.Shared.ImageFiles;
using bezdna_proto.Titanfall2.FileTypes;
using ReactiveUI;
using RPAK2L.Program.Dialogs;
using RPAK2L.Program.ViewModels.FileView.Types;
using RPAK2L.Program.ViewModels.FileView.Views;
using RPAK2L.Program.ViewModels.SubMenuViewModels;
using RPAK2L.Program.Views;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using File = System.IO.File;

namespace RPAK2L.Program.Tools
{
    public class Exporters
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">PakFileInfo</param>
        /// <param name="LastSelectedDirectory">Directory to pull from</param>
        /// <param name="exportDir">Directory to export to</param>
        /// <param name="exportSub">Sub-dir of export</param>
        /// <param name="ExportStatic">export static assets (deprecated, always true)</param>
        /// <param name="material">if is a material</param>
        /// <returns> 0 if OK, 1 if unsupported, 2 if failed</returns>
        public static int TextureData(PakFileInfo file, string LastSelectedDirectory, string exportDir, string exportSub = "", bool ExportStatic = true, bool material = false)
        {
            int _return = 0;
            
            if (file == null){
                Logger.Log.Error("File export attempt was null");
                return 2;
            }

            Logger.Log.Info($"Exporting Texture to {exportDir}");
            var tex = file.SpecificTypeFile as Texture;


            string pak = file.Pak.StarPaks[0]
                .Substring(file.Pak.StarPaks[0].LastIndexOf('\\')+1);
            
            var compression = tex.Algorithm.ToUpper();
            Logger.Log.Debug($"{compression} | 0x{tex.StartSeekRPak.ToString("X").ToUpper()}");
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


            List<string> failedImages = new List<string>();
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

                    var fs = new MemoryStream();
                    
                    if (text.streaming)
                    {
                        Logger.Log.Info("Opening starpak stream [STREAMING]");
                        FileStream spr = new FileStream(
                            Path.Combine(LastSelectedDirectory, pak),
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
                    fs.Seek(0, SeekOrigin.Begin);
                    ProgressableTask _task = new ProgressableTask(0,1);
                    _task.Init("Decoding");
                    string pngPath = filename.Replace(".dds", ".png");
                    //try
                    {
                        
                        var decoder = new BCnEncoder.Decoder.BcDecoder();
                        decoder.Options.IsParallel = false;
                        using Image<Rgba32> image = decoder.DecodeToImageRgba32(fs);

                        FileExtras.DeleteIfExists(pngPath); //the loading screen doesn't go away unless you do this?
                        var outStream = new FileStream(pngPath, FileMode.Create);
                        outStream.Seek(0, SeekOrigin.Begin);
                        image.SaveAsPng(outStream);
                    }
                    /*catch (Exception exc)
                    {
                        var rfs = File.Create(filename);
                        rfs.Write(fs.ToArray());
                        Dispatcher.UIThread.Post(() =>
                        {
                            
                            Program.AppMainWindow.WarningDialog("Could not convert to png, saved as dds");
                        });
                        rfs.Close();
                        
                        _return = 2;
                    }*/
                    _task.Finish();
                    fs.Close();
                }
                else
                {
                    Program.AppMainWindow.WarningDialog("Unsupported Compression Algoritm");
                    _return = 1;
                }
            }
            Logger.Log.Info("Finished Exporting");
            DirectoryTreeViewModel.__isLoading = false; //fuck you
            return _return;
        }
    }
}