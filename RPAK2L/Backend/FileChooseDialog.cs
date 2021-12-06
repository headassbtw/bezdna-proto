using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace RPAK2L.Backend
{
    public class FileChooseDialog
    {
        public static async Task<string> ChooseRpakDialog(string basePath)
        {
            var win = new OpenFileDialog();
            win.AllowMultiple = false;
            win.InitialFileName = basePath + "file";
            var fil = new List<FileDialogFilter>();
            fil.Add((new FileDialogFilter()
            {
                Extensions = {"rpak"},
                Name = "Respawn PAK File"
            }));
            win.Filters = fil;
            var task = win.ShowAsync(Program.AppMainWindow);
            var res = await task;
            if (res == null || res.Length <= 0)
                return null;
            else
                return res.FirstOrDefault();
        }
    }
}