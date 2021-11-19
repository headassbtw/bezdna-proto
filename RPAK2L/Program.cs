using System;
using System.IO;
using System.Reactive;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RPAK2L.ErrorReporter;
using RPAK2L.Headers;
using RPAK2L.ViewModels.FileView.Views;
using RPAK2L.Views.SubMenus;

namespace RPAK2L
{
    
    
    class ICrashLifetime : 
        IControlledApplicationLifetime,
        IApplicationLifetime
    {
        public Window MainWindow;
        public void Shutdown(int exitCode = 0)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ControlledApplicationLifetimeStartupEventArgs>? Startup;
        public event EventHandler<ControlledApplicationLifetimeExitEventArgs>? Exit;
    }
    
    class Program
    {
        public static Window AppMainWindow;
        public static HeaderInterface Headers;
        private static AppBuilder _builderInstance;
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            Logger.Log = new Logger("./Console.log");
            Headers = new HeaderInterface();
            if (OperatingSystem.IsWindows())
            {
                #pragma warning disable CA1416 //this is under a function that only runs on windows
                Console.SetWindowSize(1,1);
                #pragma warning restore CA1416
                Console.Title = "Joseph Mother";
                var handle = GetConsoleWindow();
                ShowWindow(handle, 0);
            }
            try
            {
                RxApp.DefaultExceptionHandler = new DontCrashPlease();
                _builderInstance = BuildAvaloniaApp();
                
                _builderInstance.StartWithClassicDesktopLifetime(args);
            }
            catch (Exception exc)
            {
                AppMainWindow.Close();

                Logger.Log.Error("Fatal Error:");
                Logger.Log.Error(exc);
                AppMainWindow = new AboutMenu();
            }
            Logger.Log.Info("Exiting");
            Logger.Log.Close();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}
