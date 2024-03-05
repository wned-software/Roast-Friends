using Firebase.Auth;
using Roast_Friends.Other;

namespace Roast_Friends.Views.Login;

public partial class LogInFormView : ContentPage
{
    private FirebaseAuthClient _authClient;

    public LogInFormView(FirebaseAuthClient authClient)
    {
        InitializeComponent();
        _authClient = authClient;
    }

    protected override async void OnAppearing()
    {
        if (Settings.isLoggedIn) await Shell.Current.GoToAsync("///userprofile");
        base.OnAppearing();
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;

        try
        {
            var userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);
            var token = await userCredential.User.GetIdTokenAsync(false);
            await SecureStorage.SetAsync("auth_token", token);

            await DisplayAlert("Success", "Login successful.", "OK");
            Settings.isLoggedIn = true;

            await Shell.Current.GoToAsync("///userprofile");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}