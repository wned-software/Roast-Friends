using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Plugin.Maui.Audio;

namespace Roast_Friends;

public partial class ChooseFirstPerson : ContentPage
{
    private IAudioManager audioManager;

    public ChooseFirstPerson()
    {
        InitializeComponent();
        audioManager = new AudioManager();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

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
        startMessageGrid.IsVisible = true;

        startLabel.Opacity = 0;
        startLabel.Scale = 0.5;
        startLabelShadow.Opacity = 0;
        startLabelShadow.Scale = 0.5;

        await startLabel.FadeTo(1, 100);
        var startPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("start.mp3"));
        startPlayer.Volume = 10.0;
        startPlayer.Play();
        await Task.WhenAll(
            startLabel.ScaleTo(1.25, 250, Easing.SpringOut),
            startLabelShadow.FadeTo(1, 100),
            startLabelShadow.ScaleTo(1.25, 250, Easing.SpringOut)
        );

        await Task.WhenAll(
            startLabel.ScaleTo(1, 100),
            startLabelShadow.ScaleTo(1, 100)
        );

        await Task.Delay(1000);

        await Shell.Current.GoToAsync("///question");
    }
}