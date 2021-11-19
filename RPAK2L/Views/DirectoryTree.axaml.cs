using System;
using System.Collections;
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
using RPAK2L.Tools;
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
        private TreeView _filesTree;
        private ListBox _pakitemsList;
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
            Title += "CI-";
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

                    _firstTimeShown = false;
                }
            };
        }

        private bool _firstTimeShown = true;
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            //DataContext = new DirectoryTreeViewModel();
            
            Logger.Log.Debug("InitComponent");
            
        }

        private List<PakFileInfo> _textures;
        private void DirView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            FileTypes selected = ((FileTypes) e.AddedItems[0]);
            vm = ((DirectoryTreeViewModel) DataContext);
            Logger.Log.Info($"DirTree selected {selected.Name}");
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
        private void FileView_OnSelectionChanged(object? sender, SelectionChangedEventArgs ev)
        {
            if(ev.AddedItems.Count <= 0) return;
            PakFileInfo selected = ((PakFileInfo) ev.AddedItems[0]);
            CurrentFileToExport = selected;
            vm = ((DirectoryTreeViewModel) DataContext);
            Logger.Log.Info($"Selected {selected.Name}");
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
                    inf.Size = "Maps: \n";
                    var material = selected.SpecificTypeFile as Material;
                    for (var i = 0; i < material.TextureReferences.Length; i++)
                    {
                        var e = material.TextureReferences[i];
                        var refName = "";
                        if (material.TextureReferences.Length % bezdna_proto.Titanfall2.FileTypes.Material.TextureRefName.Length == 0)
                            refName = bezdna_proto.Titanfall2.FileTypes.Material.TextureRefName[i % bezdna_proto.Titanfall2.FileTypes.Material.TextureRefName.Length];
                        else
                            refName = i < bezdna_proto.Titanfall2.FileTypes.Material.TextureRefName.Length ? bezdna_proto.Titanfall2.FileTypes.Material.TextureRefName[i] : $"UNK{i}";
                        inf.Size += refName;
                        var a = _textures
                            .FirstOrDefault(f => 
                                (f.SpecificTypeFile as Texture)
                                .GUID == e);
                        if (a != null)
                        {
                            Texture tex = a.SpecificTypeFile as Texture;
                            if(tex != null) inf.Size += " | " + tex.Width + "x" + tex.Height;
                            
                        }
                        inf.Size += '\n';
                    }
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

        }

        private bool _init;

        private Button ExportButton;
        private Button ReplaceButton;
        private Button DeleteButton;
        private Button AddButton;

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
            Logger.Log.Debug("AddButtonInit");
            AddButton = sender as Button;
            AddButton.IsEnabled = false;
        }
        
        
        private void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
        {
            this.WarningDialog("Feature not implemented");
        }
        private void ExportButton_OnClick(object? sender, RoutedEventArgs ev)
        {
            bool ExportStreaming = true;
            switch (CurrentFileToExport.File.ShortName)
            {
                case "txtr":
                    Exporters.TextureData(CurrentFileToExport, LastSelectedDirectory, "Textures");
                    break;
                case "matl":
                    var material = CurrentFileToExport.SpecificTypeFile as Material;
                    for (var i = 0; i < material.TextureReferences.Length; i++)
                    {
                        var e = material.TextureReferences[i];
                        var refName = "";
                        if (material.TextureReferences.Length % bezdna_proto.Titanfall2.FileTypes.Material.TextureRefName.Length == 0)
                            refName = bezdna_proto.Titanfall2.FileTypes.Material.TextureRefName[i % bezdna_proto.Titanfall2.FileTypes.Material.TextureRefName.Length];
                        else
                            refName = i < bezdna_proto.Titanfall2.FileTypes.Material.TextureRefName.Length ? bezdna_proto.Titanfall2.FileTypes.Material.TextureRefName[i] : $"UNK{i}";
                        var tex = _textures.FirstOrDefault(f => (f.SpecificTypeFile as Texture).GUID == e);
                        Exporters.TextureData(tex, LastSelectedDirectory, "Materials",false,true);
                    }
                    
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
            this.WarningDialog("Feature not implemented");
        }

        private PakInterface pakBackend;
        private int _lifetime;
        private void Load(string fullRPakPath)
        {
            vm.IsLoading = true;
            _lifetime++;
            Logger.Log.Debug($"Load called from lifetime {_lifetime}");
             
                
                //vm.Types = new ObservableCollection<FileTypes>();

                ThreadPool.QueueUserWorkItem(sync =>
                {
                    vm.Types.Clear();
                    pakBackend = new PakInterface(fullRPakPath);
                    FinishLoad();
                });
                
                
                
                
        }


        private void FinishLoad()
        {
            FileTypes tex = new FileTypes() {Name = "Textures"};
            FileTypes mat = new FileTypes() {Name = "Materials"};
            FileTypes sha = new FileTypes() {Name = "Shaders"};
            FileTypes dtb = new FileTypes() {Name = "DataTables"};
            FileTypes msc = new FileTypes() {Name = "Misc"};
            
            vm.Types.Add(tex);
            vm.Types.Add(mat);
            vm.Types.Add(sha);
            vm.Types.Add(dtb);
            vm.Types.Add(msc);

                vm.CurrentStarpak = "";
                Logger.Log.Debug("Starpaks:");
                foreach (string starpak in pakBackend.R2Pak.Pak.StarPaks)
                {
                    vm.CurrentStarpak += starpak + "  ";
                    Logger.Log.Debug(starpak);
                }

                for(int i = 0; i < pakBackend.R2Pak.PakInfos.Count; i++)
                {
                    
                    var file = pakBackend.R2Pak.PakInfos[i];
                    //Thread LoadThread = new Thread(async =>
                    {
                        
                        //Console.WriteLine($"Adding {file.File.StarpakOffset}");
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
                                //Console.WriteLine("unhandled file extension, throwing in misc...");
                                msc.Files.Add(file);
                                break;
                        }
                        _textures = new List<PakFileInfo>();
                        _textures.Clear();
                        foreach (var texs in tex.Files)
                        {
                            _textures.Add(texs);
                        }

                    }//);
                    //LoadThread.Name = $"{file.File.StarpakOffset} Load Thread";
                    //LoadThread.Start();
                }
                Logger.Log.Info("Finished loading");
                vm.IsLoading = false;
        }
        
        private void FileOpen_OnClick(object? sender, RoutedEventArgs e)
        {
            vm.SearchBoxFilter = "";
            _filesTree.UnselectAll();
            _pakitemsList.UnselectAll();
            
            vm.VisibleFiles.Clear();
            vm.Types.Clear();
            vm.Files.Clear();
            
            AddButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            ExportButton.IsEnabled = false;
            ReplaceButton.IsEnabled = false;
            vm.InfoName = "";
            vm.InfoBytes = "";
            vm.InfoOffset = "";
            
            
            
            FillInRpaks(iniInstance.GetValue("GamePath"));
        }

        private void FillInRpaks(string path)
        {
            if (vm == null)
                return;
            string dir = Path.Combine(path, "r2", "paks", "Win64");
            if (!Directory.Exists(dir))
            {
                this.WarningDialog("Paks folder is missing or invalid");
                return;
            }
            LastSelectedDirectory = path;
            var allpaks = Directory.GetFiles(dir);
            vm.RPakChoices.Clear();
            foreach (string pak in allpaks.Where(a => a.EndsWith(".rpak") && !a.EndsWith(").rpak")))
            {
                vm.RPakChoices.Add(pak);
            }
        }
        
        private void RPakItemControl_Initialized(object? sender, EventArgs e)
        {
            Logger.Log.Debug("RPakSelectorInit");
        }


        private void DirTree_OnInitialized(object? sender, EventArgs e)
        {
            _filesTree = sender as TreeView;
            Logger.Log.Debug("DirTreeInit");
            vm = ((DirectoryTreeViewModel) DataContext);
        }

        private void TestMenu_OnClick(object? sender, RoutedEventArgs e)
        {
            this.WarningDialog("Oh god oh fuck oh god oh fuck");
        }
        
        private void AddButton_OnClick(object? sender, RoutedEventArgs e)
        {
            
            this.WarningDialog("Feature not implemented");
        }


        private void RPakItemControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            string selected = ((string) e.AddedItems[0]);
            Logger.Log.Debug($"Selected RPAK {selected}");
            string tmp = selected.Replace('\\', '/');
            tmp = tmp.Substring(tmp.LastIndexOf('/')+1);
            tmp = tmp.Substring(0, tmp.LastIndexOf('.'));
            Console.WriteLine(tmp);
            PakName = tmp;
            Load(selected);
        }


        private void PakItems_OnInit(object? sender, EventArgs e)
        {
            _pakitemsList = sender as ListBox;
        }

        private void DebugThrowError_OnClick(object? sender, RoutedEventArgs e)
        {
            throw new Exception("User-Thrown Exception");
        }
    }
}
