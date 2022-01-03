using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reflection;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RPAK2L.Program.ErrorReporter;
using RPAK2L.Program.Headers;
using RPAK2L.Program.ViewModels.FileView.Views;
using RPAK2L.Program.Views;
using RPAK2L.Program.Views.SubMenus;

namespace RPAK2L.Program
{
    public class Program
    {

        public static Window AppMainWindow;
        public static HeaderInterface Headers;
        private static AppBuilder _builderInstance;
        public static string logPath = "";
        public static bool Updating = false;
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        
        /// <summary>
        /// Main program function
        /// </summary>
        /// <param name="args"></param>
        /// <returns>"ok" if normally run, the log path if crashed, the install folder if updating</returns>
        public static string Main(string[] args)
        {
            InitSystems();
            
            try
            {
                if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.MainWindow = new DirectoryTree();
                    desktop.MainWindow.DataContext = new DirectoryTreeViewModel(desktop.MainWindow);
                    AppMainWindow = desktop.MainWindow;
                }
                
                
                RxApp.DefaultExceptionHandler = new DontCrashPlease();
            }
            catch (Exception exc)
            {
                Logger.Log.Error("Fatal Error:");
                Logger.Log.Error(exc);
               
                
                Logger.Log.Close();
                RPAK2L.UI.Funcs.Exit(2,logPath);
                return logPath;
            }

            if (Updating) return Environment.CurrentDirectory;
            return "ok";
        }


        public static void InitSystems()
        {
            Directory.CreateDirectory("./Logs");
            bool foundAvailableLog = false;
            
            int tmp = 0;
            while (!foundAvailableLog)
            {
                string suffix = (tmp > 0) ? $"_{tmp.ToString().PadLeft(2,'0')}" : "";
                string path = Path.Combine(Environment.CurrentDirectory, "Logs",$"log_{DateTime.Now.ToString("yyyy.MM.dd.HH")}{suffix}.log");
                if (!File.Exists(path))
                {
                    logPath = path;
                    foundAvailableLog = true;
                }
                tmp++;
            }
            Settings.Init(Path.Combine(Environment.CurrentDirectory, "settings.ini"));
            Settings.Load();
            Logger.Log = new Logger(logPath);
            Headers = new HeaderInterface();
            if (OperatingSystem.IsWindows())
            {
                if (!Debugger.IsAttached) //reponse to https://github.com/headassbtw/rpak2l/issues/1
                {
                    //when compiled on linux, C# apps always show a console on windows. i don't want that.
#pragma warning disable CA1416 //this is under a function that only runs on windows
                    Console.SetWindowSize(1,1);
#pragma warning restore CA1416
                    Console.Title = "Joseph Mother";
                    var handle = GetConsoleWindow();
                    ShowWindow(handle, 0);
                }
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}
