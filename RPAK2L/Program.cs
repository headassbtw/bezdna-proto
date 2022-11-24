using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using RPAK2L.Program;
using RPAK2L.Program.Views.SubMenus;

namespace RPAK2L
{
    public class Wrapper
    {
        public static string[] Args;
        
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<RPAK2L.App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
        
        public static void Main(string[] args)
        {
            Args = args;
            RPAK2L.Common.Vars.Args = args;
            var builder = BuildAvaloniaApp();
            Program.Program.InitSystems();
            RPAK2L.Common.Funcs.Exiting += (sender, typeArgs) =>
            {
                Logger.Log.Close();
                var lifetime = (builder.Instance.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime);
                Console.WriteLine("Exiting");
                lifetime.Shutdown();
            };
            try
            {
                builder.StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("Shutting Down");
                

        }
    }
}