﻿using Avalonia;
using Avalonia.ReactiveUI;

namespace Mapsui.Samples.Avalonia;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
.StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
                       .UsePlatformDetect()
                       .With(new Win32PlatformOptions
                       {
                           EnableMultitouch = true,
                           AllowEglInitialization = true
                       })
                       .LogToTrace()
                       .UseReactiveUI();
    }
}
