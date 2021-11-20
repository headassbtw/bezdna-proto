using System;
using System.Threading;
using Avalonia.Controls;
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

        public void Init()
        {
            DirectoryTreeViewModel.__isLoading = true;
            DirectoryTreeViewModel.ProgTextRight = TotalItems.ToString();
        }

        public void Finish()
        {
            DirectoryTreeViewModel.__isLoading = false;
        }
        public void IncrementBar()
        {
            CurrentItems++;
            DirectoryTreeViewModel.ProgTextLeft = CurrentItems.ToString();
        }
    }
}