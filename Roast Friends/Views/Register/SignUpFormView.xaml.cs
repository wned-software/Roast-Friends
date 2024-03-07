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
            await DisplayAlert("B��d", "Nieprawid�owy format emaila.", "OK");
            return;
        }

        if (password.Length < 6)
        {
            await DisplayAlert("B��d", "Has�o musi zawiera� co najmniej 6 znak�w.", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("B��d", "Podane has�a nie s� identyczne.", "OK");
            return;
        }

        var userExists = await CheckIfUserExists(email, login);
        if (userExists)
        {
            await DisplayAlert("B��d", "Email lub nazwa u�ytkownika ju� istnieje.", "OK");
            return;
        }

        try
        {
            var userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);
            var uid = userCredential.User.Uid;

            await AddUserToDatabase(uid, login, email);

            await DisplayAlert("Sukces", "Rejestracja zako�czona pomy�lnie.", "OK");
            await Shell.Current.GoToAsync("///loginformview");
        }
        catch (Exception ex)
        {
            await DisplayAlert("B��d", ex.Message, "OK");
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