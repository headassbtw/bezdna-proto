using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;
using RPAK2L.ViewModels.FileView.Views;

namespace RPAK2L.Dialogs
{
    /// <summary>
    /// an action or task that would be useful to have a progress bar for, such as file operations that take a while
    /// designed to be inherited
    /// </summary>
    public class ProgressableTask
    {

        public int CurrentItems;
        public int TotalItems;
        public ProgressableTask(int current, int total)
        {
            CurrentItems = current;
            TotalItems = total;
        }

        public void Init(string purpose)
        {
            DirectoryTreeViewModel._instance.IsLoading = true;
            DirectoryTreeViewModel.TaskProgress = 0;
            Dispatcher.UIThread.Post(() =>
            {
                DirectoryTreeViewModel._instance._centerGrid.Height = 70;
                DirectoryTreeViewModel.ProgTextMid = purpose;
                DirectoryTreeViewModel._instance._bar.IsIndeterminate = false;
                DirectoryTreeViewModel._instance._bar.Maximum = TotalItems;
            });
            DirectoryTreeViewModel.ProgTextRight = TotalItems.ToString();
        }

        public void Finish()
        {
            ThreadPool.QueueUserWorkItem(async =>
            {
                Dispatcher.UIThread.Post(() =>
                {

                    DirectoryTreeViewModel._instance._centerGrid.Opacity = 0;
                });
                Thread.Sleep(400);
                Dispatcher.UIThread.Post(() =>
                {    
                    DirectoryTreeViewModel._instance.IsLoading = false;
                });
            });
        }

        public void IncrementBar(int items)
        {
            
            CurrentItems = items;
            Dispatcher.UIThread.Post(() =>
            {
                DirectoryTreeViewModel._instance._bar.Value = items;
            });
            DirectoryTreeViewModel.ProgTextLeft = items.ToString();
        }
        public void IncrementBar()
        {
            CurrentItems++;
            DirectoryTreeViewModel.ProgTextLeft = CurrentItems.ToString();
        }
    }
}