using Firebase.Auth.Providers;
using Firebase.Auth;

namespace Roast_Friends.Views;

public partial class ForgotPassword : ContentPage
{
    public ForgotPassword(FirebaseAuthClient firebaseAuthClient)
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;

        
    }
}