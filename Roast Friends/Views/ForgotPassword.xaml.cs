using Firebase.Auth;
using FirebaseAdmin.Auth;

namespace Roast_Friends.Views;

public partial class ForgotPassword : ContentPage
{
    private FirebaseAuthClient _firebaseAuthClient;
    public ForgotPassword(FirebaseAuthClient firebaseAuthClient)
	{
		InitializeComponent();
        _firebaseAuthClient = firebaseAuthClient;
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        _firebaseAuthClient.ResetEmailPasswordAsync(email);
    }
}