using System;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using RPAK2L.Headers;
using RPAK2L.ViewModels.FileView.Views;

namespace RPAK2L
{
    class Program
    {
        public static Window AppMainWindow;
        public static HeaderInterface Headers;
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            FileStream ostrm;
            StreamWriter writer;
            TextWriter oldOut = Console.Out;
            try
            {
                ostrm = new FileStream("./Console.log", FileMode.Create, FileAccess.ReadWrite);
                writer = new StreamWriter(ostrm);
                //Console.SetOut(writer);
                Console.WriteLine("Logger redirect initialized");

            }
            catch (Exception exc)
            {
                Console.WriteLine("Logger redirect file could not be opened:");
                Console.WriteLine(exc.Message);
            }
            
            
            
            
            
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
            
            
            
            
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
            Console.WriteLine("Exiting");
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}
