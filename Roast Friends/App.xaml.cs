using Roast_Friends;
using Roast_Friends.Other;

namespace Roast_Friends
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
            InitializeApp();
        }

        private async void InitializeApp()
        {
            await TryAutoLoginAsync();
        }

        private async Task TryAutoLoginAsync()
        {
            var authToken = await SecureStorage.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(authToken))
            {
                Settings.isLoggedIn = true;
            }
            else
            {
                Settings.isLoggedIn = false;
            }
        }
    }
}