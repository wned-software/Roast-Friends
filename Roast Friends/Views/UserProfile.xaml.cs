using Firebase.Database;
using Firebase.Database.Query;
using Roast_Friends.Other;
namespace Roast_Friends.Views;

public partial class UserProfile : ContentPage
{
    private FirebaseClient _firebaseClient;
    public UserProfile()
    {
        InitializeComponent();
        _firebaseClient = new FirebaseClient(
                Settings.FireBaseDatabaseUrl,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(Settings.FireBaseSecretKey)
                });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUserProfile();
    }

    private async Task LoadUserProfile()
    {
        var userLogin = await SecureStorage.GetAsync("user_login");
        if (!string.IsNullOrEmpty(userLogin))
        {
            ProfileNameLabel.Text = $"PROFIL: {userLogin}";

            var uid = await SecureStorage.GetAsync("user_uid");
            if (!string.IsNullOrEmpty(uid))
            {
                var userCounter = await _firebaseClient
                    .Child("roastfriends")
                    .Child("users")
                    .Child(uid)
                    .Child("counter")
                    .OnceSingleAsync<int>();
                UnlockedQuestionsLabel.Text = Convert.ToString(userCounter);
            }
        }
        else
        {
            await Shell.Current.GoToAsync("///loginformview");
        }
    }


    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await SecureStorage.SetAsync("auth_token", string.Empty);
        await SecureStorage.SetAsync("user_login", string.Empty);
        Settings.isLoggedIn = false;
        await Shell.Current.GoToAsync("///StartPage");
    }
}