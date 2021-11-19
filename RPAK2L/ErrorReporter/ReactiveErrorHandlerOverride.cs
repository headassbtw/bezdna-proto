using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using ReactiveUI;

namespace RPAK2L.ErrorReporter
{
    public class DontCrashPlease : IObserver<Exception>
    {
        public void OnNext(Exception value)
        {
            if (Debugger.IsAttached) Debugger.Break();

            RxApp.MainThreadScheduler.Schedule(() => { Logger.Log.Error("i tried to handle the error"); });
        }

        public void OnError(Exception error)
        {
            if (Debugger.IsAttached) Debugger.Break();

            RxApp.MainThreadScheduler.Schedule(() => { Logger.Log.Error("i tried to handle the error"); });
        }

        public void OnCompleted()
        {
            if (Debugger.IsAttached) Debugger.Break();
            RxApp.MainThreadScheduler.Schedule(() => { Logger.Log.Error("i tried to handle the error"); });
        }
    }
}