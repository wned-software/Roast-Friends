using Firebase.Auth;

namespace Roast_Friends.Views.Login;

public partial class LogInFormView : ContentPage
{
    private FirebaseAuthClient _authClient;
    public LogInFormView(FirebaseAuthClient authClient)
    {
        InitializeComponent();
        _authClient = authClient;
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;

        try
        {
            var userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);
            await DisplayAlert("Success", "Login successful.", "OK");

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}