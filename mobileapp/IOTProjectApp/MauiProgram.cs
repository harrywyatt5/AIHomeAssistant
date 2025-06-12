using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace IOTProjectApp;

public static class MauiProgram
{
    public static ServiceManager ServiceManager { get; private set; } = ServiceManager.Singleton;
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.UseMauiApp<App>().UseMauiCommunityToolkit();
        return builder.Build();
    }
}