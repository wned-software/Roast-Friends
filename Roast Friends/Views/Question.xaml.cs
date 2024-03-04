using CommunityToolkit.Maui.Alerts;
using Firebase.Database;
using Firebase.Database.Query;
using Roast_Friends.Other;
using System.Linq;
using System.Threading.Tasks;

namespace Roast_Friends;

public partial class Question : ContentPage
{
    private FirebaseClient firebaseClient;
    public int counter { get; set; } = 0;
    private const string CounterKey = "counter";
    public QuestionModel SelectedQuestion { get; set; }
    

    public Question()
    {
        InitializeComponent();
        firebaseClient = new FirebaseClient(
            Settings.FireBaseDatabaseUrl,
            new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(Settings.FireBaseSecretKey) });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        counter = Preferences.Get(CounterKey, 0);
        if (counter > 20)
        {
            Toast.Make("Przenoszenie...", CommunityToolkit.Maui.Core.ToastDuration.Short, 20).Show();
            await Shell.Current.GoToAsync("///StartPage");
        }
        await FetchQuestion();
        
    }

    private async Task FetchQuestion()
    {
        counter++;
        var allQuestions = await firebaseClient
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
            await Shell.Current.GoToAsync("///StartPage");
        }
        Preferences.Set(CounterKey, counter);
        Toast.Make("counter: " + counter, CommunityToolkit.Maui.Core.ToastDuration.Long, 20).Show();
        await Shell.Current.GoToAsync("///giveNextPerson");

    }
}