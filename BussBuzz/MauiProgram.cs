using AutoFixture;
using BussBuzz.MVVM.MAP.ViewModell;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Maps;
using Material.Components.Maui.Extensions;
using Material.Components.Maui.Tokens;
using Microsoft.Extensions.Logging;

namespace BussBuzz
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiMaps()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMaps("YOUR_KEY")
                .RegisterAppServices()
                .UseMaterialComponents(
                new List<string>
                {
                    "Roboto-Regular.ttf",
                    "Roboto-Italic.ttf",
                    "Roboto-Medium.ttf",
                    "Roboto-MediumItalic.ttf",
                    "Roboto-Bold.ttf",
                    "Roboto-BoldItalic.ttf",
                }
            );
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<Microsoft.Maui.Controls.Maps.Map, CustomMapHandler>();
            });

            FontMapper.AddFont("OpenSans-Regular.ttf", "OpenSans");

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }

        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<IFixture, Fixture>();
            return mauiAppBuilder;
        }
    }
}