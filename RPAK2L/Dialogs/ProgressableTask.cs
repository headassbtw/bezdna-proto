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
                DirectoryTreeViewModel.ProgTextMid = purpose;
                DirectoryTreeViewModel._instance._bar.IsIndeterminate = false;
                DirectoryTreeViewModel._instance._bar.Maximum = TotalItems;
            });
            DirectoryTreeViewModel.ProgTextRight = TotalItems.ToString();
        }

        public void Finish()
        {
            //fuck you
            Dispatcher.UIThread.Post(() =>
            {    
                DirectoryTreeViewModel._instance.IsLoading = false;
            });
        }

        public void IncrementBar(int items)
        {
            
            CurrentItems = items;
            Dispatcher.UIThread.Post(() =>
            {
                DirectoryTreeViewModel._instance._bar.Value = items;
                DirectoryTreeViewModel.ProgTextLeft = items.ToString();
            });
            
        }
        public void IncrementBar()
        {
            CurrentItems++;
            Dispatcher.UIThread.Post(() =>
            {
                DirectoryTreeViewModel.ProgTextLeft = CurrentItems.ToString();
            });
        }
    }
}
