using Firebase.Database;
using Firebase.Database.Query;
using Roast_Friends.Other;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Roast_Friends.Models;

namespace Roast_Friends.Views;

public partial class UserProfile : ContentPage
{
    private FirebaseClient _firebaseClient;
    public bool IsAdmin { get; set; } = false;

    public UserProfile()
    {
        InitializeComponent();
        _firebaseClient = new FirebaseClient(
                Settings.FireBaseDatabaseUrl,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(Settings.FireBaseSecretKey)
                });
        this.BindingContext = this;
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
                var userDetails = await _firebaseClient
                    .Child("roastfriends")
                    .Child("users")
                    .Child(uid)
                    .OnceSingleAsync<UserModel>();

                UnlockedQuestionsLabel.Text = userDetails.counter.ToString();
                IsAdmin = userDetails.permissions == "admin";
                OnPropertyChanged(nameof(IsAdmin));
            }
        }
        else
        {
            await Shell.Current.GoToAsync("///loginformview");
        }
    }

    private async void CheckQuestions(object sender, EventArgs e)
    {
        if (IsAdmin)
        {
            await Shell.Current.GoToAsync("///checkquestions");
        }
        else
        {
            await DisplayAlert("Access Denied", "You do not have permission to view this page.", "OK");
        }
    }

    private async void AddQuestion(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///addquestion");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await SecureStorage.SetAsync("auth_token", string.Empty);
        await SecureStorage.SetAsync("user_login", string.Empty);
        Settings.isLoggedIn = false;
        await Shell.Current.GoToAsync("///StartPage");
    }

    private void gobackArrow_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("///StartPage");
    }
}