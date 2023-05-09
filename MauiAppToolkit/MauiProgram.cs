using MauiAppToolkit.Views;
using Microsoft.Extensions.Logging;

namespace MauiAppToolkit;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
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
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
    {
        // Only one view model
//        mauiAppBuilder.Services.AddSingleton<ViewModels.ViewModelBase>();
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
