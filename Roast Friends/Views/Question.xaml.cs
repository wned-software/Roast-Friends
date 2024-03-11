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
    public QuestionModel? SelectedQuestion { get; set; }

    public int userCounter;

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
                await DisplayAlert("Informacja", "Nie masz już więcej darmowych pytań", "OK");
                await Shell.Current.GoToAsync("//useaccountinfo");
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
                    userCounter = await _firebaseClient
                        .Child("roastfriends")
                        .Child("users")
                        .Child(uid)
                        .Child("counter")
                        .OnceSingleAsync<int>();

                    if (!(userCounter <= 0))
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
                    await DisplayAlert("Błąd", "Proszę zalogować się ponownie", "OK");
                    await Shell.Current.GoToAsync("//loginformview");
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
        if (Settings.isLoggedIn)
        {
            if (userCounter <= 0)
            {
                await DisplayAlert("Informacja", "Nie masz już więcej pytań.", "OK");
                await Shell.Current.GoToAsync("//userprofile");
            } else await Shell.Current.GoToAsync("//giveNextPerson");
        } else {
            if (counter > 20)
            {
                await Shell.Current.GoToAsync("//useaccountinfo");
            } else await Shell.Current.GoToAsync("//giveNextPerson");
            Preferences.Set(CounterKey, counter);
        }

    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        if (Settings.isLoggedIn) await Shell.Current.GoToAsync("//userprofile");
        else await Shell.Current.GoToAsync("//notloggedin");
    }

    private async void gobackArrow_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//StartPage");
    }
}