using System.Reflection;
using RPAK2L.Program.ViewModels;
using ReactiveUI;

namespace RPAK2L.Program.Dialogs.HelpDialog
{
    public class HelpContext : ReactiveObject
    {
        public string Header => "Help";
        public string VersionText => $"v{Assembly.GetExecutingAssembly().GetName().Version}";
    }
}