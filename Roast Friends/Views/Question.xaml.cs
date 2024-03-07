using CommunityToolkit.Maui.Alerts;
using Firebase.Database;
using Firebase.Database.Query;
using Roast_Friends.Other;
using System.Linq;
using System.Threading.Tasks;

namespace Roast_Friends;

public partial class Question : ContentPage
{
    private FirebaseClient _firebaseClient;
    public int counter { get; set; } = 0;
    private const string CounterKey = "counter";
    public QuestionModel SelectedQuestion { get; set; }
    

    public Question(FirebaseClient firebaseclient)
    {
        InitializeComponent();
        _firebaseClient = firebaseclient;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!Settings.isLoggedIn == true)
        {
            counter = Preferences.Get(CounterKey, 0);
            if (counter > 20)
            {
                Toast.Make("Przenoszenie...", CommunityToolkit.Maui.Core.ToastDuration.Short, 20).Show();
                await Shell.Current.GoToAsync("///useaccountinfo");
            }
            else
            {
                counter++;
                await FetchQuestion();
            }
        }
        else
        {
            try
            {
                //jest zalogowany
                var uid = await SecureStorage.GetAsync("user_uid");

                if (uid != null)
                {
                    Toast.Make(uid, CommunityToolkit.Maui.Core.ToastDuration.Long, 5).Show();
                    int userCounter = await _firebaseClient
                        .Child("roastfriends")
                        .Child("users")
                        .Child(uid)
                        .Child("counter")
                        .OnceSingleAsync<int>();

                    if (userCounter < 0)
                    {
                        await DisplayAlert("Informacja", "Nie masz już więcej pytań.", "OK");
                        await Shell.Current.GoToAsync("///userprofile");
                    }
                    else
                    {
                        userCounter--;
                        await FetchQuestion();
                        await _firebaseClient
                            .Child("roastfriends")
                            .Child("users")
                            .Child(uid)
                            .Child("counter")
                            .PutAsync(userCounter);
                    }
                }
                else
                {
                    await DisplayAlert("Informacja", "UID użytkownika jest pusty.", "OK");
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("a", e.Message, "OK");
            }

        }
    }

    private async Task FetchQuestion()
    {

        var allQuestions = await _firebaseClient
            .Child("roastfriends")
            .Child("question")
            .OnceAsync<QuestionModel>();

        var checkedQuestions = allQuestions
            .Where(q => q.Object.Status == "CHECKED")
            .Select(q => q.Object)
            .ToList();

        if (checkedQuestions.Any())
        {
            var random = new Random();
            SelectedQuestion = checkedQuestions[random.Next(checkedQuestions.Count)];

            QuestionLabel.Text = SelectedQuestion.Value;
            QuestionLabelShadow.Text = SelectedQuestion.Value;
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (counter > 20)
        {
            Toast.Make("Przenoszenie...", CommunityToolkit.Maui.Core.ToastDuration.Short, 20).Show();
            await Shell.Current.GoToAsync("///useaccountinfo");
        }
        Preferences.Set(CounterKey, counter);
        Toast.Make("counter: " + counter, CommunityToolkit.Maui.Core.ToastDuration.Long, 20).Show();
        await Shell.Current.GoToAsync("///giveNextPerson");

    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        if (Settings.isLoggedIn) await Shell.Current.GoToAsync("///userprofile");
        else await Shell.Current.GoToAsync("///notloggedin");
    }
}