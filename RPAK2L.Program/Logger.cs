using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RPAK2L.Program.ViewModels.FileView.Views;

namespace RPAK2L.Program
{
    public class InAppConsoleMsg
    {
        public string TextLine { get; set; }
        public string HexColorCode { get; set; }
    }
    public class Logger
    {
        public static Logger Log; 
        public StreamWriter LogFileWriter;
        public Logger(string logFilePath)
        {
            LogFileWriter = new StreamWriter(new FileStream(logFilePath, FileMode.Create, FileAccess.ReadWrite));
            string infover = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
            string build = infover.Substring(infover.IndexOf('+') + 1);
            LogFileWriter.WriteLine($"RPAK2L " +
                                    #if RELEASE
                                    "Release" +
                                    #elif DEBUG
                                    "Debug" +
                                    #elif EXTREME_DEBUG
                                    "Extreme Debug" +
                                    #endif
                                    $" Build {build}");
            LogFileWriter.WriteLine($"Running on {RuntimeInformation.OSDescription}");
            LogFileWriter.WriteLine("Logger Initialized");
            LogFileWriter.WriteLine();
        }

        private void CommonLog(string log, string logType, string fileName, int lineNumber, ConsoleColor color)
        {
            Console.ForegroundColor = color;

            string inf =
                    "[" + logType 
            #if DEBUG || EXTREME_DEBUG
                        + $" {fileName} Line {lineNumber}"
            #endif
                + "] " + log;
            Console.WriteLine(inf);
            LogFileWriter.WriteLine(inf);
            Console.ResetColor();
            string _hexcol = "#FFFFFF";
            switch (color)
            {
                case ConsoleColor.Red:
                    _hexcol = "#FF4444";
                    break;
                case ConsoleColor.White:
                    _hexcol = "#FFFFFF";
                    break;
                case ConsoleColor.DarkGreen:
                    _hexcol = "#009900";
                    break;
                case ConsoleColor.Yellow:
                    _hexcol = "#999900";
                    break;
            }
            DirectoryTreeViewModel.ConsoleLog(
                new InAppConsoleMsg()
                {
                    TextLine = $"{logType}] {log}",
                    HexColorCode = _hexcol
                }
            );
        }
        
        public void Close()
        {
            LogFileWriter.Close();
        }
        public void Info(object log, [CallerFilePath] string CallerFile = "", [CallerLineNumber] int LineNumber = 0)
        {
            CommonLog(log.ToString(),"Info",CallerFile,LineNumber, ConsoleColor.White);
        }

        public void Warning(object log, [CallerFilePath] string CallerFile = "", [CallerLineNumber] int LineNumber = 0)
        {
            CommonLog(log.ToString(),"Warning",CallerFile,LineNumber, ConsoleColor.Yellow);
        }
        public void Error(object log, [CallerFilePath] string CallerFile = "", [CallerLineNumber] int LineNumber = 0)
        {
            CommonLog(log.ToString(),"Error",CallerFile,LineNumber, ConsoleColor.Red);
        }
        public void Debug(object log, [CallerFilePath] string CallerFile = "", [CallerLineNumber] int LineNumber = 0)
        {
            CommonLog(log.ToString(),"Debug",CallerFile,LineNumber, ConsoleColor.DarkGreen);
        }
    }
}