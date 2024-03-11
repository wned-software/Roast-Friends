using Firebase.Auth;
namespace Roast_Friends.Views;

public partial class ForgotPassword : ContentPage
{
    private FirebaseAuthClient _firebaseAuthClient;
    public ForgotPassword(FirebaseAuthClient firebaseAuthClient)
	{
		InitializeComponent();
        _firebaseAuthClient = firebaseAuthClient;
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        _firebaseAuthClient.ResetEmailPasswordAsync(email);
        await DisplayAlert("Informacja", "Na podany adres email przesłano link do resetowania hasła", "OK");
        await Shell.Current.GoToAsync("///loginformview");
    }
}