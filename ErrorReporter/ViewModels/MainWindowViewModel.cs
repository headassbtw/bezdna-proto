using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Avalonia;
using ReactiveUI;

namespace ErrorReporter.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public bool HasError => Program.HasProgram;
        public string Header => $"{Program.ProgramName} Has crashed.";
        public string Report => "Report";
        public string Cancel => "Cancel";
        public ReactiveCommand<uint,uint> ReportCommand { get; set; }
        public ReactiveCommand<uint,uint> CancelCommand { get; set; }
        public string LogFile { get; }

        public MainWindowViewModel()
        {
            LogFile = Program.Log;
            ReportCommand = ReactiveCommand.Create<uint,uint>(u =>
            {
                //TODO: report it somehow idk
                OpenUrl("https://github.com/headassbtw/rpak2l/issues/new?assignees=&labels=bug&template=bug_report.md&title=");
                return 0;
            });
            CancelCommand = ReactiveCommand.Create<uint,uint>(u =>
            {
                Environment.Exit(0);
                return 0;
            });
        }
        
        private void OpenUrl(string url)
        {
            try
            {
                var p = Process.Start(url);
                
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
            Thread.Sleep(200);
            Environment.Exit(0);
        }

        
    }
}