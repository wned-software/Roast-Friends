using Firebase.Database;
using Firebase.Database.Query;
using Roast_Friends.Other;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Roast_Friends.Models;
using System.ComponentModel;

namespace Roast_Friends.Views;

public partial class UserProfile : ContentPage, INotifyPropertyChanged
{
    private FirebaseClient _firebaseClient;
    private bool _isAdmin;

    public bool IsAdmin
    {
        get => _isAdmin;
        set
        {
            if (_isAdmin == value) return;
            _isAdmin = value;
            OnPropertyChanged(nameof(IsAdmin));
        }
    }

    public UserProfile(FirebaseClient firebaseClient)
    {
        InitializeComponent();
        _firebaseClient = firebaseClient;
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

                UnlockedQuestionsLabel.Text = userDetails.counter;
                IsAdmin = userDetails.permissions == "admin";
            }
        }
        else
        {
            await Shell.Current.GoToAsync("///loginformview");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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

    private async void gobackArrow_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///StartPage");
    }
}