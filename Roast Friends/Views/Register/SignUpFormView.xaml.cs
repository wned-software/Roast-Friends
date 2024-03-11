using Firebase.Auth;
using Firebase.Database;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Firebase.Database.Query;
using System.Diagnostics.Metrics;

namespace Roast_Friends.Views.Register;

public partial class SignUpFormView : ContentPage
{
    private FirebaseAuthClient _authClient;
    private FirebaseClient _firebaseClient;

    public SignUpFormView(FirebaseAuthClient authClient, FirebaseClient firebaseClient)
    {
        InitializeComponent();
        _authClient = authClient;
        _firebaseClient = firebaseClient;
    }

    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;
        string confirmPassword = ConfirmPasswordEntry.Text;
        string login = LoginEntry.Text;

        if (!IsValidEmail(email))
        {
            await DisplayAlert("Błąd", "Nieprawidłowy format emaila.", "OK");
            return;
        }

        if (password.Length < 6)
        {
            await DisplayAlert("Błąd", "Hasło musi zawierać co najmniej 6 znaków.", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Błąd", "Podane hasła nie są identyczne.", "OK");
            return;
        }

        var userExists = await CheckIfUserExists(email, login);
        if (userExists)
        {
            await DisplayAlert("Błąd", "Email lub nazwa użytkownika już istnieje.", "OK");
            return;
        }

        try
        {
            var userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);
            var uid = userCredential.User.Uid;

            await AddUserToDatabase(uid, login, email);

            await DisplayAlert("Sukces", "Rejestracja zakończona pomyślnie.", "OK");
            await Shell.Current.GoToAsync("//loginformview");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Błąd", ex.Message, "OK");
        }
    }

    private async Task<bool> CheckIfUserExists(string email, string login)
    {
        var emailCheck = await _firebaseClient
            .Child("roastfriends")
            .Child("users")
            .OrderBy("email")
            .EqualTo(email)
            .OnceAsync<object>();
        if (emailCheck.Count > 0) return true;

        var loginCheck = await _firebaseClient
            .Child("roastfriends")
            .Child("users")
            .OrderBy("login")
            .EqualTo(login)
            .OnceAsync<object>();
        if (loginCheck.Count > 0) return true;

        return false;
    }

    private async Task AddUserToDatabase(string uid, string login, string email)
    {
        int cnt = 20 - Preferences.Get("counter", 0);
        await _firebaseClient
            .Child("roastfriends")
            .Child("users")
            .Child(uid)
            .PutAsync(new { login = login, email = email, counter = cnt, permissions = "user" });
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private async void gobackArrow_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//notloggedin");
    }
}