using Firebase.Auth;
using Firebase.Database;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Firebase.Database.Query;

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
            await DisplayAlert("B³¹d", "Nieprawid³owy format emaila.", "OK");
            return;
        }

        if (password.Length < 6)
        {
            await DisplayAlert("B³¹d", "Has³o musi zawieraæ co najmniej 6 znaków.", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("B³¹d", "Podane has³a nie s¹ identyczne.", "OK");
            return;
        }

        var userExists = await CheckIfUserExists(email, login);
        if (userExists)
        {
            await DisplayAlert("B³¹d", "Email lub nazwa u¿ytkownika ju¿ istnieje.", "OK");
            return;
        }

        try
        {
            var userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);
            var uid = userCredential.User.Uid;

            await AddUserToDatabase(uid, login, email);

            await DisplayAlert("Sukces", "Rejestracja zakoñczona pomyœlnie.", "OK");
            await Shell.Current.GoToAsync("///loginformview");
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", ex.Message, "OK");
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
        await _firebaseClient
            .Child("roastfriends")
            .Child("users")
            .Child(uid)
            .PutAsync(new { login = login, email = email, counter = 0, permissions = "user" });
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
}