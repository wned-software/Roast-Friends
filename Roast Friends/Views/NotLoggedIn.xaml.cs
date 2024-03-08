namespace Roast_Friends;

public partial class NotLoggedIn : ContentPage
{
	public NotLoggedIn()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///signupformview");
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///loginformview"); 
    }

    private async void gobackArrow_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///StartPage");
    }
}