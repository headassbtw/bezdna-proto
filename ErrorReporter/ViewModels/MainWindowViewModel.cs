using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
                Environment.Exit(0);
                return 0;
            });
            CancelCommand = ReactiveCommand.Create<uint,uint>(u =>
            {
                Environment.Exit(0);
                return 0;
            });
        }
    }
}