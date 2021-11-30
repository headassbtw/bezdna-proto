using ReactiveUI;
using RPAK2L.ViewModels;

namespace RPAK2L.Dialogs
{
    public class WarningMultiItemDataContext : ViewModelBase
    {
        private string _imgLocation = "";
        private string _warning = "";
        private string[] _content = new string[0];

        public string Image
        {
            get => _imgLocation;
            set
            {
                this.RaiseAndSetIfChanged(ref _imgLocation, value);
            }
        }
        
        public string Warning
        {
            get => _warning;
            set
            {
                this.RaiseAndSetIfChanged(ref _warning, value);
            }
        }

        public string[] Content
        {
            get => _content;
            set
            {
                this.RaiseAndSetIfChanged(ref _content, value);
            }
        }
    }
}