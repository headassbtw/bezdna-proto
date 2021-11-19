using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using RPAK2L.Dialogs;

namespace RPAK2L.Backend
{
    public static class DirectoryChooseDialog
    {
        #nullable enable
        public static async Task<string>? ShowDirDialog(this Window parentWindow, string windowTitle, string[]? mustIncludeSubDir = null)
        {
            var diag = new OpenFolderDialog();
            diag.Title = windowTitle;
            string task = await diag.ShowAsync(parentWindow);
            
            if (task == null) return null; //the user canceled the dialog
            else if (Directory.Exists(task))
            {
                bool boolin = (mustIncludeSubDir != null && mustIncludeSubDir.Length > 0);
                bool bigboolin = mustIncludeSubDir != null && Directory.Exists(Path.Combine(task, Path.Combine(mustIncludeSubDir)));
                if(boolin)
                {
                    if (bigboolin) return task;
                    else
                    {
                        parentWindow.WarningDialog($"Folder is invalid, must contain \"{Path.Combine(mustIncludeSubDir)}\".");
                        return null;
                    }
                }
                else return task;
            }
            else
            {
                parentWindow.WarningDialog("Path does not exist.");
                return null;
            }
        }
        
        

        public static void ShowDirDialogSync(this Window parentWindow, string windowTitle, Control textBox, string[]? mustIncludeSubDir = null)
        {
            var task = parentWindow.ShowDirDialog(windowTitle, mustIncludeSubDir);
            var retn = "";
            var q = ThreadPool.QueueUserWorkItem(async str =>
            {
                while (!task.IsCompleted)
                {
                    Thread.Sleep(10);
                }

                str = task.Result;
                
                Dispatcher.UIThread.Post(() =>
                {
                    if(!string.IsNullOrEmpty(task.Result))
                        (textBox as TextBox).Text = task.Result;
                });
                
            });
        }
    }
}