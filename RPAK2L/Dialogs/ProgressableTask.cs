using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using RPAK2L.ViewModels.FileView.Views;

namespace RPAK2L.Dialogs
{
    public class ProgressableTaskStartedEventArgs : EventArgs
    {
        public ProgressableTaskStartedEventArgs(int items)
        {
            TotalItems = items;
        }
        public int TotalItems;
    }
    /// <summary>
    /// an action or task that would be useful to have a progress bar for, such as file operations that take a while
    /// designed to be inherited
    /// </summary>
    public abstract class ProgressableTask : ReactiveObject
    {
        private TaskProgressDialog _dialog;
        private int _processed;

        public int ProcessedItems
        {
            get => _processed;
            set
            {
                this.RaiseAndSetIfChanged(ref _processed, value);
            }
        }
        private int _totalItems;

        public int TotalItems
        {
            get => _totalItems;
            set
            {
                this.RaiseAndSetIfChanged(ref _totalItems, value);
            }
        }
        public Action IncrementProgress;
        public EventHandler<ProgressableTaskStartedEventArgs> Started;

        public ProgressableTask()
        {
            
            Started += StartedHit;
        }

        public void Init(int totalItems)
        {
            TotalItems = totalItems;
            Init();
        }
        public void Init()
        {
            ProcessedItems = 0;
            IncrementProgress = () =>
            {
                Console.WriteLine($"{ProcessedItems}/{TotalItems}");
                ProcessedItems++;
            };
            ShowWindow();
            
        }

        public abstract void Run();
        private void StartedHit(object? sender, ProgressableTaskStartedEventArgs e)
        {
            ShowWindow();
        }
        bool _finished = false;
        private void ShowWindow()
        {
            _dialog = new TaskProgressDialog();
            _dialog.DataContext = this;
            var diag = _dialog.ShowDialog(Program.AppMainWindow);
            _dialog.Activated += (sender, args) =>
            {
                ThreadPool.QueueUserWorkItem(async =>
                {
                    Run();
                    Console.WriteLine("Task finished");
                    _finished = true;
                    Dispatcher.UIThread.Post(() =>
                    {
                        _dialog.Close();
                    });
                });
            };
            
            
        }
    }
}