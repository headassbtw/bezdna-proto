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
            var task = await diag.ShowAsync(parentWindow);
            bool boolin = mustIncludeSubDir is string[] sd;
            if (Directory.Exists(task))
            {
                if (boolin && Directory.Exists(Path.Combine(task, Path.Combine(mustIncludeSubDir))))
                    return task;
                else
                {
                    parentWindow.WarningDialog($"Folder is invalid, must contain \"{Path.Combine(mustIncludeSubDir)}\".");
                    return null;
                }
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