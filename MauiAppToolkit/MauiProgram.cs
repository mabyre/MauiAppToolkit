using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui.Media;
using Camera.MAUI;
using MauiAppToolkit.Views;

namespace MauiAppToolkit;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder()
			.UseMauiApp<App>()
            .UseMauiCameraView()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("fa_solid.ttf", "FontAwesome");
            })
            .RegisterViewModels()
            .RegisterView();

        RegisterServices(builder.Services);

#if DEBUG
        builder.Logging.AddDebug().SetMinimumLevel(LogLevel.Trace);
#endif

		return builder.Build();
	}

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<ViewModels.BaseViewModel>();
        mauiAppBuilder.Services.AddSingleton<ViewModels.ConsoleViewModel>(); 
        mauiAppBuilder.Services.AddSingleton<ViewModels.FileViewModel>();
        mauiAppBuilder.Services.AddTransient<ViewModels.SpeechToTextViewModel>();

        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterView(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<ConsolePage>();
        mauiAppBuilder.Services.AddSingleton<FilePage>();
        mauiAppBuilder.Services.AddSingleton<SpeechToTextPage>();

        return mauiAppBuilder;
    }

    static void RegisterServices(in IServiceCollection services)
    {
        services.AddSingleton<ISpeechToText>(SpeechToText.Default);
        services.AddSingleton<ITextToSpeech>(TextToSpeech.Default);
    }
}
