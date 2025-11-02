using Microsoft.Extensions.Logging;
using PermitManagement.Presentation;
using PermitManagement.Presentation.Interfaces;
using System.Net.Http;

namespace PermitManagement.Maui;

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
			});

		builder.Services.AddSingleton<HttpClient>(_ => new HttpClient
		{
			BaseAddress = new Uri("https://localhost:7158")
		});
		builder.Services.AddSingleton<IPermitApiClient, PermitApiClient>();
		builder.Services.AddSingleton<PermitViewModel>();
		builder.Services.AddSingleton<MainPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
