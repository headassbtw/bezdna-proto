using ReactiveUI;
using RPAK2L.Program.ViewModels;

namespace RPAK2L.Program.Dialogs
{
    public class WarningDataContext : ViewModelBase
    {
        private string _imgLocation = "";
        private string _content = "";

        public string Image
        {
            get => _imgLocation;
            set
            {
                this.RaiseAndSetIfChanged(ref _imgLocation, value);
            }
        }

        public string Content
        {
            get => _content;
            set
            {
                this.RaiseAndSetIfChanged(ref _content, value);
            }
        }
    }
}