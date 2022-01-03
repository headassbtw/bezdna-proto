using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace ErrorReporter;


public static class FLExt
{
    public static int StartWithFuckLifetime<T>(
        this T builder,
        string[] args,
        ShutdownMode shutdownMode = ShutdownMode.OnLastWindowClose)
        where T : AppBuilderBase<T>, new()
    {
        FuckLifetime lifetime = new FuckLifetime()
        {
            Args = args,
            ShutdownMode = shutdownMode
        };
        builder.SetupWithLifetime((IApplicationLifetime) lifetime);
        return lifetime.Start(args);
    }
}
public class FuckLifetime : ClassicDesktopStyleApplicationLifetime
{
    
}