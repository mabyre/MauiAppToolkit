using MauiAppToolkit.Views;
using Microsoft.Extensions.Logging;

namespace MauiAppToolkit;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder()
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("fa_solid.ttf", "FontAwesome");
            })
            .RegisterViewModels()
            .RegisterView();

#if DEBUG
		builder.Logging.AddDebug().SetMinimumLevel(LogLevel.Trace);
#endif

		return builder.Build();
	}

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<ViewModels.BaseViewModel>();
        mauiAppBuilder.Services.AddSingleton<ViewModels.ConsoleViewModel>(); 
        mauiAppBuilder.Services.AddSingleton<ViewModels.MainViewModel>();

        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterView(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<ConsolePage>();
        mauiAppBuilder.Services.AddSingleton<MainPage>();

        return mauiAppBuilder;
    }
}
