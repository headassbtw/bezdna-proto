using System.Reactive;
using ReactiveUI;
using RPAK2L.Dialogs;

namespace RPAK2L.ViewModels.SubMenuViewModels
{
    public class AboutMenuViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> ShitOnXboxCommand { get; set; }
        public AboutMenuViewModel()
        {
            ShitOnXboxCommand = ReactiveCommand.Create(() => {Program.AppMainWindow.WarningDialog("CONSIDER YOUR XBOX SHAT ON NERDDDDD"); });
        }
    }
}