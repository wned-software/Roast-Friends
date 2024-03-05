using Plugin.Maui.Audio;

namespace Roast_Friends;

public partial class MainPage : ContentPage
{
    private readonly IAudioManager audioManager;

    public MainPage(IAudioManager audioManager)
    {
        InitializeComponent();
        StartPulsingAnimation();
        this.audioManager = audioManager;
    }
    private async void Button_Clicked(object sender, EventArgs e)
    {
        var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("klik.mp3"));
        player.Volume = 10.0;
        player.Play();

        await Shell.Current.GoToAsync("///loginformview");
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