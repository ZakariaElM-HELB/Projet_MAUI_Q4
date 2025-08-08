using CommunityToolkit.Maui;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using System.Xml;

namespace MyApp
{
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
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<MainViewModel>();

            builder.Services.AddTransient<DetailsView>();
            builder.Services.AddTransient<DetailsViewModel>();

            builder.Services.AddSingleton<DeviceOrientationService>();
            builder.Services.AddSingleton<JSONServices>();
            builder.Services.AddSingleton<CSVServices>(); // <--- AJOUTE CETTE LIGNE

            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<HomeView>();

            builder.Services.AddTransient<AddMemberViewModel>(); // Enregistrement du ViewModel
            builder.Services.AddTransient<AddMemberView>(); // Enregistrement de la page

            // 📊 Enregistrement pour ChartView
            builder.UseMicrocharts();

            return builder.Build();
        }
    }
}
