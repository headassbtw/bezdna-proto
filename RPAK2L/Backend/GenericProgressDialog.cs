using System.Threading;
using RPAK2L.Dialogs;

namespace RPAK2L.Backend
{
    public class GenericProgressDialog : ProgressableTask
    {
        private bool _done = false;

        public void Finish()
        {
            _done = true;
        }
        
        public override void Run()
        {
            while(!_done)
                Thread.Sleep(10);
        }
    }
}