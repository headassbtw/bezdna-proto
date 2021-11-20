using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using DynamicData;

namespace ErrorReporter
{
    class Program
    {
        public static string ProgramName;

        public static string LogPath;
        public static string Log;

        public static bool HasLog;
        public static bool HasProgram;
        
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Contains("-p"))
            {
                ProgramName = args[args.IndexOf("-p") + 1];
            }
            if (args.Contains("-path"))
            {
                LogPath = args[args.IndexOf("-path") + 1];
            }

            HasProgram = (args.Contains("-p"));
            HasLog = !string.IsNullOrEmpty(LogPath);
            if (HasLog)
            {
                try
                {
                    Log = File.ReadAllText(LogPath);
                }
                catch (Exception exc)
                {
                    Log = $"Error while reading log:\n\n{exc.Message}";
                }
            }
            else
            {
                Log = $"No Log at:\n{LogPath}";
            }
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
            
            
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}