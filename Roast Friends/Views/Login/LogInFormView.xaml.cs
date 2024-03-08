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
                        await DisplayAlert("Błąd", "Nie znaleziono takiego użytkownika. Sprawdź swoje dane albo załóż nowe konto.", "OK");
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
                await SecureStorage.SetAsync("user_uid", userCredential.User.Uid);

                await DisplayAlert("Sukces", "Logowanie zakończone sukcesem.", "OK");
                Settings.isLoggedIn = true;

                await Shell.Current.GoToAsync("///userprofile");
            }
            catch (FirebaseAuthException ex)
            {
                string errorMessage = "Wystąpił błąd podczas logowania. Spróbuj ponownie później.";
                switch (ex.Reason)
                {
                    case AuthErrorReason.WrongPassword:
                        errorMessage = "Hasło jest niepoprawne. Spróbuj jeszcze raz.";
                        break;
                    case AuthErrorReason.UnknownEmailAddress:
                    case AuthErrorReason.UserNotFound:
                        errorMessage = "Nie znaleziono takiego użytkownika. Sprawdź swoje dane albo załóż nowe konto.";
                        break;
                    case AuthErrorReason.TooManyAttemptsTryLater:
                        errorMessage = "Zbyt wiele nieudanych prób logowania. Spróbuj ponownie później.";
                        break;
                }
                await DisplayAlert("Error", errorMessage, "OK");
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Wystąpił nieoczekiwany błąd. Spróbuj ponownie później.", "OK");
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }
    }
}