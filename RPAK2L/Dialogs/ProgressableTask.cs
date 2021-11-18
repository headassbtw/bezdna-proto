using System;
using System.Threading;
using Avalonia.Controls;
using ReactiveUI;

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
        public int ProcessedItems;
        private int _totalItems;
        private double _percentage;
        public double Percentage
        {
            get => _percentage;
            set
            {
                this.RaiseAndSetIfChanged(ref _percentage, value);
            }
        }

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
            IncrementProgress = () =>
            {
                ProcessedItems++;
                if (ProcessedItems != 0 && TotalItems != 0)
                    Percentage = ProcessedItems / TotalItems;
            };
            Started += StartedHit;
        }

        public void Init()
        {
            ShowWindow();
            
        }

        public abstract void Run();
        private void StartedHit(object? sender, ProgressableTaskStartedEventArgs e)
        {
            ShowWindow();
        }

        private void ShowWindow()
        {
            _dialog = new TaskProgressDialog();
            _dialog.DataContext = this;
            _dialog.ShowDialog(Program.AppMainWindow);
            _dialog.Activated += async (sender, args) => {Run(); };
        }
    }
}