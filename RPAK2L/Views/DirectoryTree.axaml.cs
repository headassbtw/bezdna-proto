using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
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
using RPAK2L.Dialogs;
using RPAK2L.ViewModels.FileView.Types;
using RPAK2L.ViewModels.FileView.Views;
using RPAK2L.ViewModels.SubMenuViewModels;
using RPAK2L.Views.SubMenus;
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
            
            
            
            this.Title = "RPAK2L";
            #if DEBUG
            this.Title += " | Debug";
            #elif EXTREME_DEBUG
            this.Title += " | Extreme Debug";
            #endif
            string infover = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
            this.Title += " | Build ";
            #if CI
            Title += "CI- ";
            #else
            Title += "LC-";
            #endif
            Title += infover.Substring(infover.IndexOf('+')+1);
            this.Activated += (sender, args) =>
            {
                if (_firstTimeShown)
                {
                    vm = DataContext as DirectoryTreeViewModel;
                    Program.Headers.Init();
                    
                    iniInstance.Load();
                    string dir = iniInstance.GetValue("GameDirectory", "","null");
                    if (dir != "null")
                    {
                        FillInRpaks(dir);
                    }
                    /*var listBox = this.FindControl<TreeView>("PakFiles");
                    listBox.GetObservable(ListBox.ScrollProperty)
                        .OfType<ScrollViewer>()
                        .Take(1)
                        .Subscribe(xv =>
                    {
                        Console.WriteLine("A");
                    });*/


                    _firstTimeShown = false;
                }
            };
        }

        private bool _firstTimeShown = true;
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            //DataContext = new DirectoryTreeViewModel();
            
            Console.WriteLine("InitComponent");
            
        }
        
        private void DirView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            FileTypes selected = ((FileTypes) e.AddedItems[0]);
            vm = ((DirectoryTreeViewModel) DataContext);
            Console.WriteLine($"Selected {selected.Name}");
            vm.Files.Clear();
            vm.SetPakFiles(selected.Files);
            vm.AddPakFiles(selected.Files.Count);
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

        private string PakName;
        private PakFileInfo CurrentFileToExport;
        private void FileView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            PakFileInfo selected = ((PakFileInfo) e.AddedItems[0]);
            CurrentFileToExport = selected;
            vm = ((DirectoryTreeViewModel) DataContext);
            Console.WriteLine($"Selected {selected.Name}");
            var inf = new Models.Inf();
            
            switch (selected.File.ShortName)
            {
                case "txtr":
                    inf.Size = "";
                    foreach (var mip in ((Texture)selected.SpecificTypeFile).TextureDatas)
                    {
                        inf.Size += mip.width + "x" + mip.height + "|" + (mip.streaming ? "Streaming" : "Static") + '|' + mip.seek.ToString("X").PadLeft(16, '0')  + '\n';
                    }
                    break;
                case "matl":
                    break;
                case "shdr":
                    break;
                case "dtbl":
                    break;
                default:
                    break;
            }
            

            
            inf.Name = selected.Name;
            
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
            Console.WriteLine("AddButtonInit");
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
                    if (tex.TextureDatas.Where(t => !t.streaming).ToList().Count > 0)
                    {
                        this.WarningDialog("Unable to access this texture (not implemented)");
                        break;
                    }
                        
                    Console.WriteLine(tex.StarpakNum);
                    var compression = tex.Algorithm.ToUpper();
                        Console.WriteLine(compression);
                    Console.WriteLine(tex.BaseFile.StarpakOffset);
                    string ex = Path.Combine(Environment.CurrentDirectory, "Export", tex.Name);
                    Directory.CreateDirectory(ex);
                    
                    foreach (var text in tex.TextureDatas)
                    {
                        if(compression == "DXT1" || compression.StartsWith("BC"))
                        {
                            Console.WriteLine($"ExportingMipMap ({text.width}x{text.height})");
                            byte[] buf = new byte[text.size];
                            var fs = File.Create(Path.Combine(ex, text.height + ".dds"));
                            Console.WriteLine("Opening starpak stream");
                            var pak = text.streaming ? "pc_stream.starpak" : $"{PakName}.rpak";
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
                            this.WarningDialog("Unsupported Compression Algoritm");
                        }
                    }
                    
                    
                    break;
                case "matl":
                    var mat = CurrentFileToExport.SpecificTypeFile as Material;
                    this.WarningDialog("Materials coming soon");
                    break;
                case "shdr":
                    var sha = CurrentFileToExport.SpecificTypeFile as Shader;
                    this.WarningDialog("Shaders not implemented");
                    break;
                case "dtbl":
                    var dtb = CurrentFileToExport.SpecificTypeFile as DataTables;
                    this.WarningDialog("DataTables not implemented");
                    break;
                default:
                    this.WarningDialog("Unknown file type");
                    break;
            }
        }
        private void ReplaceButton_OnClick(object? sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private int _lifetime;
        private void Load(string fullRPakPath)
        {
            _lifetime++;
            Console.WriteLine($"Load called from lifetime {_lifetime}");
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
                
                //vm.Types = new ObservableCollection<FileTypes>();
                
                Console.WriteLine("t1");
                var pakBackend = new PakInterface(fullRPakPath);
                Console.WriteLine("t2");

                Console.WriteLine("Starpaks:");
                foreach (string starpak in pakBackend.R2Pak.Pak.StarPaks)
                {
                    Console.WriteLine(starpak);
                }

                Console.WriteLine("Begginning Load...");

                for(int i = 0; i < pakBackend.R2Pak.PakInfos.Count; i++)
                {
                    
                    var file = pakBackend.R2Pak.PakInfos[i];
                    //Thread LoadThread = new Thread(async =>
                    {
                        
                        Console.WriteLine($"Adding {file.File.StarpakOffset}");
                        switch (file.File.ShortName)
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
                        }
                    }//);
                    //LoadThread.Name = $"{file.File.StarpakOffset} Load Thread";
                    //LoadThread.Start();
                }
                Console.WriteLine("Finished loading");
        }
        
        //private Backend.PakInterface pakBackend;
        private void FileOpen_OnClick(object? sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new OpenFolderDialog();
            dialog.Directory = (LastSelectedDirectory == null)
                ? System.IO.Directory.GetCurrentDirectory()
                : LastSelectedDirectory;
            dialog.Title = "Game Folder";
            vm = DataContext as DirectoryTreeViewModel;
            //vm = Program.VM;
            
            
            Thread fileOpenThread = new Thread(async =>
            {
                var task = dialog.ShowAsync(this);
                task.Wait();
                if (task.IsCompletedSuccessfully && task.Result.FirstOrDefault() != null)
                {
                    string path = task.Result;
                    FillInRpaks(path);

                    //Load();
                }
                else
                {
                    //error or sumn
                }

            });
            fileOpenThread.Start();
        }

        private void FillInRpaks(string path)
        {
            if (vm == null)
                return;
            LastSelectedDirectory = path;
            var allpaks = Directory.GetFiles(Path.Combine(path, "r2","paks","Win64"));

            foreach (string pak in allpaks.Where(a => a.EndsWith(".rpak") && !a.EndsWith(").rpak")))
            {
                vm.RPakChoices.Add(pak);
            }
        }
        
        private void RPakItemControl_Initialized(object? sender, EventArgs e)
        {
            Console.WriteLine("RPakSelectorInit");
            
        }


        private void DirTree_OnInitialized(object? sender, EventArgs e)
        {
            Console.WriteLine("DirTreeInit");
            Console.WriteLine("ASS");
            Console.WriteLine((DataContext as DirectoryTreeViewModel));
            vm = ((DirectoryTreeViewModel) DataContext);
        }

        private void TestMenu_OnClick(object? sender, RoutedEventArgs e)
        {
            this.WarningDialog("Oh god oh fuck oh god oh fuck");
        }
        
        private void AddButton_OnClick(object? sender, RoutedEventArgs e)
        {
            
            throw new NotImplementedException("ASS");
        }


        private void RPakItemControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            string selected = ((string) e.AddedItems[0]);
            Console.WriteLine($"Selected RPAK {selected}");
            string tmp = selected.Replace('\\', '/');
            tmp = tmp.Substring(tmp.LastIndexOf('/')+1);
            tmp = tmp.Substring(0, tmp.LastIndexOf('.'));
            Console.WriteLine(tmp);
            PakName = tmp;
            Load(selected);
        }

        
    }
}
