using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ErrorReporter.ViewModels;
using RPAK2L.Program;
using RPAK2L.Program.Views.SubMenus;

namespace RPAK2L
{
    public class Wrapper
    {
        
        
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<RPAK2L.App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
        
        public static void Main(string[] args)
        {
            var builder = BuildAvaloniaApp();
            Program.Program.InitSystems();
            RPAK2L.UI.Funcs.Exiting += (sender, typeArgs) =>
            {
                var lifetime = (builder.Instance.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime);
                Console.WriteLine("Exiting");
                switch (typeArgs.Type)
                {
                    
                    case 0: //normal
                        lifetime.Shutdown();
                        lifetime.Dispose();
                        break;
                    case 1: //updating
                        Updater.Main.Update(Environment.CurrentDirectory);
                        break;
                    case 2: //crashed
                        var mw = new ErrorReporter.Views.MainWindow();
                        mw.DataContext = new MainWindowViewModel();
                        ErrorReporter.Program.ProgramName = "RPAK2L";
                        ErrorReporter.Program.HasProgram = true;
                        ErrorReporter.Program.HasLog = true;
                        Program.Logger.Log.Close();
                        ErrorReporter.Program.Log = "File.ReadAllText(typeArgs.Parameters)";
                        Console.WriteLine(typeArgs.Parameters);
                        lifetime.MainWindow.Close();
                        lifetime.MainWindow = mw;
                        lifetime.MainWindow.Show();
                        break;
                }
                lifetime.Dispose();
            };
            try
            {
                builder.StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                RPAK2L.UI.Funcs.Exit(2, Program.Program.logPath);
            }
            
                string rtn = "";
            //string rtn = Program.Program.Main(args);


            Thread.Sleep(500);
            Console.WriteLine(rtn);
            if (File.Exists(rtn))
            {
                Console.WriteLine("is a file, showing crash");
                ErrorReporter.Program.Main(new []{
                    "-p",
                    "RPAK2L",
                    "-path",
                    rtn
                });
            }

            if (Directory.Exists(rtn))
            {
                Console.WriteLine("is a directory, updating");
                //ThreadPool.QueueUserWorkItem(task => Updater.Main.Update(rtn));
                Updater.Main.Update(rtn);
            }

        }
    }
}