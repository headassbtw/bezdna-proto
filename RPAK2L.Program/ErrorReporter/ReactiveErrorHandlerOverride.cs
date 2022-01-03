using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using ReactiveUI;
using RPAK2L.Program.Dialogs;

namespace RPAK2L.Program.ErrorReporter
{
    public class DontCrashPlease : IObserver<Exception>
    {
        public void OnNext(Exception value)
        {
            if (Debugger.IsAttached) Debugger.Break();

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Logger.Log.Error("Error from ReactiveCommand:"); 
                Logger.Log.Error(value);
                Program.AppMainWindow.WarningDialog("An error was thrown, but it could be handled. nothing was done.");
                
            });
        }

        public void OnError(Exception error)
        {
            if (Debugger.IsAttached) Debugger.Break();

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Logger.Log.Error("Error from ReactiveCommand:");
                Logger.Log.Error(error);
                
            });
        }

        public void OnCompleted()
        {
            if (Debugger.IsAttached) Debugger.Break();
            RxApp.MainThreadScheduler.Schedule(() => { Logger.Log.Error("i tried to handle the error"); });
        }
    }
}