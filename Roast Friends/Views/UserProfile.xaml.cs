using Roast_Friends.Other;
namespace Roast_Friends.Views;

public partial class UserProfile : ContentPage
{
    public UserProfile()
    {
        InitializeComponent();
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await SecureStorage.SetAsync("auth_token", string.Empty);
        Settings.isLoggedIn = false;
        await Shell.Current.GoToAsync("///StartPage");
    }
}