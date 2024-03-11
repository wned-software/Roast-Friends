using Plugin.Maui.Audio;

namespace Roast_Friends;

public partial class GiveNextPerson : ContentPage
{
    private IAudioManager audioManager;

    public GiveNextPerson()
    {
        InitializeComponent();
        audioManager = new AudioManager();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        initialMessageGrid.IsVisible = true;
        await Task.Delay(3000);

        initialMessageGrid.IsVisible = false;
        countdownGrid.IsVisible = true;

        for (int i = 5; i > 0; i--)
        {
            var klikPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("klik.mp3"));
            klikPlayer.Volume = 10.0;
            klikPlayer.Play();

            countdownLabel.Text = i.ToString();
            countdownLabelShadow.Text = i.ToString();

            countdownLabel.Opacity = 0;
            countdownLabel.Scale = 0.5;
            countdownLabelShadow.Opacity = 0;
            countdownLabelShadow.Scale = 0.5;

            await Task.WhenAll(
                countdownLabel.FadeTo(1, 200),
                countdownLabel.ScaleTo(1.2, 500),
                countdownLabelShadow.FadeTo(1, 200),
                countdownLabelShadow.ScaleTo(1.2, 500)
            );

            await Task.WhenAll(
                countdownLabel.FadeTo(0, 500),
                countdownLabel.ScaleTo(0.5, 500),
                countdownLabelShadow.FadeTo(0, 500),
                countdownLabelShadow.ScaleTo(0.5, 500)
            );
        }

        countdownGrid.IsVisible = false;

        await Shell.Current.GoToAsync("//question");
    }
}