using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace LotCoMPrinter;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("Aptos.ttf", "AptosRegular");
				fonts.AddFont("Aptos-SemiBold.ttf", "AptosSemiBold");
			});
#if DEBUG
		builder.Logging.AddDebug();
#endif
		return builder.Build();
	}
}
