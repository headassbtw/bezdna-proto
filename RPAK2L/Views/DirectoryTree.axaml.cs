using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.X11;
using bezdna_proto.Titanfall2.FileTypes;
using DynamicData.Binding;
using ImageMagick;
using ImageMagick.Formats;
using RPAK2L.Backend;
using RPAK2L.ViewModels.FileView.Types;
using RPAK2L.ViewModels.FileView.Views;
using File = System.IO.File;
using Path = System.IO.Path;

namespace RPAK2L.Views
{
    public class DirectoryTree : Window
    {
        private DirectoryTreeViewModel vm;
        public DirectoryTree()
        {
            
            InitializeComponent();
            
#if DEBUG
            this.AttachDevTools();
#endif
            
        }
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            //DataContext = new DirectoryTreeViewModel();
            vm = DataContext as DirectoryTreeViewModel;
        }
        
        private void DirView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            FileTypes selected = ((FileTypes) e.AddedItems[0]);
            vm = ((DirectoryTreeViewModel) DataContext);
            Console.WriteLine($"Selected {selected.Name}");
            vm.Files.Clear();
            foreach(var thign in selected.Files)
                vm.Files.Add(thign);
        }

        private Ini iniInstance = new Ini(System.IO.Path.Combine(Environment.CurrentDirectory, "settings.ini"));
        private string LastSelectedDirectory
        {
            get
            {
                iniInstance.Load();
                string path = iniInstance.GetValue("GameDirectory");
                if (path == null) path = Environment.CurrentDirectory;
                return path;
            }
            set
            {
                iniInstance.Load();
                iniInstance.WriteValue("GameDirectory", value);
                iniInstance.Save();
            }
        }
        private PakFileInfo CurrentFileToExport;
        private void FileView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            PakFileInfo selected = ((PakFileInfo) e.AddedItems[0]);
            CurrentFileToExport = selected;
            vm = ((DirectoryTreeViewModel) DataContext);
            Console.WriteLine($"Selected {selected.Name}");
            var inf = new Models.Inf();
            /*
            switch (selected.File.ShortName)
            {
                case "txtr":
                    tex.Files.Add(file);
                    break;
                case "matl":
                    mat.Files.Add(file);
                    break;
                case "shdr":
                    sha.Files.Add(file);
                    break;
                case "dtbl":
                    dtb.Files.Add(file);
                    break;
                default:
                    Console.WriteLine("unhandled file extension, throwing in misc...");
                    msc.Files.Add(file);
                    break;
            }*/
            

            
            inf.Name = selected.Name;
            inf.Size = $"{selected.File.Count} Bytes";
            vm.FileInfo = inf;
            AddButton.IsEnabled = false;
            DeleteButton.IsEnabled = true;
            ExportButton.IsEnabled = true;
            ReplaceButton.IsEnabled = true;
            if (!_init)
            {
                _init = true;
                ExportButton.IsEnabled = true;
                ReplaceButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
            vm.InfoName = inf.Name;
            vm.InfoBytes = inf.Size;
            vm.InfoOffset = inf.Offset;
            
            NameBlock.Text = inf.Name;
            ByteBlock.Text = inf.Size;
            OffsetBlock.Text = inf.Offset;

        }

        private bool _init;
        private TextBlock NameBlock;
        private TextBlock ByteBlock;
        private TextBlock OffsetBlock;

        private Button ExportButton;
        private Button ReplaceButton;
        private Button DeleteButton;
        private Button AddButton;

        private void NameLabel_OnInitialized(object? sender, EventArgs e)
        {
            NameBlock = sender as TextBlock;
        }

        private void ByteLabel_OnInitialized(object? sender, EventArgs e)
        {
            ByteBlock = sender as TextBlock;
        }
        private void OffsetLabel_OnInitialized(object? sender, EventArgs e)
        {
            OffsetBlock = sender as TextBlock;
        }

        private void DeleteButton_OnInitialized(object? sender, EventArgs e)
        {
            DeleteButton = sender as Button;
            DeleteButton.IsEnabled = false;
        }
        private void ExportButton_OnInitialized(object? sender, EventArgs e)
        {
            ExportButton = sender as Button;
            ExportButton.IsEnabled = false;
        }
        private void ReplaceButton_OnInitialized(object? sender, EventArgs e)
        {
            ReplaceButton = sender as Button;
            ReplaceButton.IsEnabled = false;
        }
        private void AddButton_OnInitialized(object? sender, EventArgs e)
        {
            AddButton = sender as Button;
            AddButton.IsEnabled = false;
        }
        
        
        private void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void ExportButton_OnClick(object? sender, RoutedEventArgs e)
        {
            switch (CurrentFileToExport.File.ShortName)
            {
                case "txtr":
                    Console.WriteLine("Exporting Texture...");
                    var tex = CurrentFileToExport.SpecificTypeFile as Texture;
                    Console.WriteLine(tex.StarpakNum);
                    var compression = tex.Algorithm.ToUpper();
                        Console.WriteLine(compression);
                    Console.WriteLine(tex.BaseFile.StarpakOffset);
                    string ex = Path.Combine(Environment.CurrentDirectory, "Export", tex.Name);
                    Directory.CreateDirectory(ex);
                    
                    foreach (var text in tex.TextureDatas)
                    {
                        if(text.height > 512)
                        {
                            Console.WriteLine($"ExportingMipMap ({text.height}x)");
                            byte[] buf = new byte[text.size];
                        var fs = File.Create(Path.Combine(ex, text.height + ".dds"));
                        Console.WriteLine("Opening pc_stream.starpak stream");
                        FileStream spr = new FileStream(
                            Path.Combine(LastSelectedDirectory, "r2", "paks", "Win64", "pc_stream.starpak"),
                            FileMode.Open);

                        spr.Seek(text.seek, SeekOrigin.Begin);
                        spr.Read(buf);
                        fs.Write(Headers.HeaderInterface.Get(tex.Height, compression));
                        fs.Write(buf);
                        fs.Close();
                        }
                        else
                        {
                            Console.WriteLine("Texture is smaller than 512x, not exporting");
                        }
                    }
                    
                    
                    break;
                case "matl":
                    var mat = CurrentFileToExport.SpecificTypeFile as Material;
                    break;
                case "shdr":
                    var sha = CurrentFileToExport.SpecificTypeFile as Shader;
                    break;
                case "dtbl":
                    var dtb = CurrentFileToExport.SpecificTypeFile as DataTables;
                    break;
                default:
                    break;
            }
        }
        private void ReplaceButton_OnClick(object? sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }


        private void Load(string fullRPakPath)
        {
             FileTypes tex = new FileTypes() {Name = "Textures"};
                    FileTypes mat = new FileTypes() {Name = "Materials"};
                    FileTypes sha = new FileTypes() {Name = "Shaders"};
                    FileTypes dtb = new FileTypes() {Name = "DataTables"};
                    FileTypes msc = new FileTypes() {Name = "Misc"};
                    vm.Types.Clear();
                    vm.Types.Add(tex);
                    vm.Types.Add(mat);
                    vm.Types.Add(sha);
                    vm.Types.Add(dtb);
                    vm.Types.Add(msc);
                    
                    vm.Types = new ObservableCollection<FileTypes>();


                    pakBackend = new PakInterface(fullRPakPath);


                    Console.WriteLine("Starpaks:");
                    foreach (string starpak in pakBackend.R2Pak.Pak.StarPaks)
                    {
                        Console.WriteLine(starpak);
                    }

                    Console.WriteLine("Begginning Load...");

                    for(int i = 0; i < pakBackend.R2Pak.PakInfos.Count; i++)
                    {
                        
                        var file = pakBackend.R2Pak.PakInfos[i];
                        Thread LoadThread = new Thread(async =>
                        {
                            
                            Console.WriteLine($"Adding {file.File.StarpakOffset}");
                            switch (file.File.ShortName)
                            {
                                case "txtr":
                                    tex.Files.Add(file);
                                    return;
                                case "matl":
                                    mat.Files.Add(file);
                                    return;
                                case "shdr":
                                    sha.Files.Add(file);
                                    return;
                                case "dtbl":
                                    dtb.Files.Add(file);
                                    return;
                                default:
                                    Console.WriteLine("unhandled file extension, throwing in misc...");
                                    msc.Files.Add(file);
                                    return;
                            }
                        });
                        LoadThread.Name = $"{file.File.StarpakOffset} Load Thread";
                        LoadThread.Start();
                    }
        }
        
        private Backend.PakInterface pakBackend;
        private void FileOpen_OnClick(object? sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.AllowMultiple = false;
            dialog.Directory = (LastSelectedDirectory == null)
                ? System.IO.Directory.GetCurrentDirectory()
                : LastSelectedDirectory;
            dialog.Title = "Game Executable";
            List<FileDialogFilter> filters = new List<FileDialogFilter>();
            FileDialogFilter filter = new FileDialogFilter();
            filter.Name = "Game Executable (*.exe)";
            filter.Extensions.Add("exe");
            filters.Add(filter);
            
            dialog.Filters = filters;
            vm = DataContext as DirectoryTreeViewModel;
            //vm = Program.VM;
            
            
            Thread fileOpenThread = new Thread(async =>
            {
                var task = dialog.ShowAsync(this);
                task.Wait();
                if (task.IsCompletedSuccessfully && task.Result.FirstOrDefault() != null)
                {
                    string path = task.Result.FirstOrDefault().Substring(0, task.Result.FirstOrDefault().LastIndexOfAny(new[] {'\\', '/'})+1);
                    LastSelectedDirectory = path;
                    var allpaks = Directory.GetFiles(Path.Combine(path, "r2","paks","Win64"));

                    foreach (string pak in allpaks.Where(a => a.EndsWith(".rpak") && !a.EndsWith(").rpak")))
                    {
                        vm.RPakChoices.Add(pak);
                    }


                    //Load();
                }
                else
                {
                    //error or sumn
                }

            });
            fileOpenThread.Start();
        }

        private void DirTree_OnInitialized(object? sender, EventArgs e)
        {
            
            Console.WriteLine("ASS");
            Console.WriteLine((DataContext as DirectoryTreeViewModel));
            vm = ((DirectoryTreeViewModel) DataContext);
        }
        
        private void AddButton_OnClick(object? sender, RoutedEventArgs e)
        {
            throw new NotImplementedException("ASS");
        }


        private void RPakItemControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            string selected = ((string) e.AddedItems[0]);
            Load(selected);
        }
    }
}
