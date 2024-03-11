using Firebase.Database;
using Firebase.Database.Query;
using Plugin.Maui.Audio;
using Roast_Friends.Other;

namespace Roast_Friends;

public partial class MainPage : ContentPage
{
    private readonly IAudioManager audioManager;
    private FirebaseClient _firebaseClient;
    public MainPage(IAudioManager audioManager, FirebaseClient firebaseClient)
    {
        InitializeComponent();
        StartPulsingAnimation();
        _firebaseClient = firebaseClient;
        this.audioManager = audioManager;
    }
    private async void Button_Clicked(object sender, EventArgs e)
    {
        var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("klik.mp3"));
        player.Volume = 10.0;
        player.Play();

        var current = Connectivity.NetworkAccess;

        if (current == NetworkAccess.Internet) {
            if (Settings.isLoggedIn)
            {

                var uid = await SecureStorage.GetAsync("user_uid");
                int userCounter = await _firebaseClient
                    .Child("roastfriends")
                    .Child("users")
                    .Child(uid)
                    .Child("counter")
                    .OnceSingleAsync<int>();

                if (userCounter <= 0) await Shell.Current.GoToAsync("///userprofile");
                else await Shell.Current.GoToAsync("///chooseFirstPerson");

            }
            else
            {
                if (Preferences.Get("counter", 0) <= 0) await Shell.Current.GoToAsync("///useaccountinfo");
                else await Shell.Current.GoToAsync("///chooseFirstPerson");
            }

        } else
        {
            await DisplayAlert("Error", "Brak połączenia do internetu", "OK");
        }
       
    }

    private void StartPulsingAnimation()
    {
        const double normalFontSize = 80;
        const double maxFontSize = 90;
        const uint animationTime = 1000;

        Action<double> updateFontSize = (v) =>
        {
            StartLabel.FontSize = v;
            StartLabelShadow.FontSize = v;
        };

        var animation = new Animation
    {
        { 0, 0.5, new Animation(v => updateFontSize(v), normalFontSize, maxFontSize, Easing.SinInOut) },
        { 0.5, 1, new Animation(v => updateFontSize(v), maxFontSize, normalFontSize, Easing.SinInOut) }
    };

        animation.Commit(this, "PulsingAnimation", 16, 2 * animationTime, Easing.Linear, null, () => true);
    }
}