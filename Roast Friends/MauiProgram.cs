using CommunityToolkit.Maui;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using Roast_Friends.Other;
using Roast_Friends.Views.Login;
using Roast_Friends.Views.Register;

namespace Roast_Friends
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
                    fonts.AddFont("Chaser_Regular.ttf", "font2");
                    fonts.AddFont("stencilla.ttf", "font1");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton(AudioManager.Current);
            builder.Services.AddTransient<MainPage>();

            builder.Services.AddSingleton(new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = Settings.FireBaseWebApiKey,
                AuthDomain = Settings.AuthDomain,
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            }));

            builder.Services.AddSingleton<FirebaseClient>(new FirebaseClient(
                Settings.FireBaseDatabaseUrl,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(Settings.FireBaseSecretKey)
                }));

            builder.Services.AddTransient<SignUpFormView>();
            builder.Services.AddTransient<LogInFormView>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}