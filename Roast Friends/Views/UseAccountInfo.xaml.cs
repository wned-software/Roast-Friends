using Roast_Friends.Other;

namespace Roast_Friends;

public partial class UseAccountInfo : ContentPage
{
	public UseAccountInfo()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (Settings.isLoggedIn) await Shell.Current.GoToAsync("//userprofile");
        else await Shell.Current.GoToAsync("//notloggedin");
    }
}