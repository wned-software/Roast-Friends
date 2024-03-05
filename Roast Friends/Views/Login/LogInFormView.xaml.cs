using Firebase.Auth;
using Firebase.Database;
using Roast_Friends.Other;
using Roast_Friends.Models;
using Firebase.Database.Query;

namespace Roast_Friends.Views.Login
{
    public partial class LogInFormView : ContentPage
    {
        private FirebaseAuthClient _authClient;
        private FirebaseClient _firebaseClient;

        public LogInFormView(FirebaseAuthClient authClient)
        {
            InitializeComponent();
            _authClient = authClient;
            _firebaseClient = new FirebaseClient(
                Settings.FireBaseDatabaseUrl,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(Settings.FireBaseSecretKey)
                });
        }

        protected override async void OnAppearing()
        {
            if (Settings.isLoggedIn) await Shell.Current.GoToAsync("///userprofile");
            base.OnAppearing();
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            string identifier = EmailEntry.Text;
            string password = PasswordEntry.Text;

            try
            {
                string email = identifier;

                if (!identifier.Contains("@"))
                {
                    var users = await _firebaseClient
                        .Child("roastfriends")
                        .Child("users")
                        .OnceAsync<UserModel>();

                    var userWithEmail = users.FirstOrDefault(u => u.Object.login == identifier)?.Object.email;

                    if (userWithEmail == null)
                    {
                        await DisplayAlert("Error", "User not found. Please check your login or register.", "OK");
                        return;
                    }

                    email = userWithEmail;
                }

                var userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);
                var token = await userCredential.User.GetIdTokenAsync(false);
                await SecureStorage.SetAsync("auth_token", token);

                var uid = userCredential.User.Uid;

                var user = await _firebaseClient
                    .Child("roastfriends")
                    .Child("users")
                    .Child(uid)
                    .OnceSingleAsync<UserModel>();

                await SecureStorage.SetAsync("user_login", user.login);

                await DisplayAlert("Success", "Login successful.", "OK");
                Settings.isLoggedIn = true;

                await Shell.Current.GoToAsync("///userprofile");
            }
            catch (FirebaseAuthException ex)
            {
                string errorMessage = "An error occurred during login. Please try again.";
                switch (ex.Reason)
                {
                    case AuthErrorReason.WrongPassword:
                        errorMessage = "The password is incorrect. Please try again.";
                        break;
                    case AuthErrorReason.UnknownEmailAddress:
                    case AuthErrorReason.UserNotFound:
                        errorMessage = "The user was not found. Please check your login details or register.";
                        break;
                    case AuthErrorReason.TooManyAttemptsTryLater:
                        errorMessage = "Too many unsuccessful login attempts. Please try again later.";
                        break;
                }
                await DisplayAlert("Error", errorMessage, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An unexpected error occurred. Please try again.", "OK");
            }
        }
    }
}