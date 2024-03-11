using Firebase.Database;
using Firebase.Database.Query;
using Roast_Friends.Models;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Roast_Friends.Other;

namespace Roast_Friends.Views;

public partial class CheckQuestions : ContentPage
{
    private FirebaseClient _firebaseClient;
    public ObservableCollection<QuestionModel> Questions { get; set; }

    public CheckQuestions(FirebaseClient firebaseClient)
    {
        InitializeComponent();
        _firebaseClient = firebaseClient;
        Questions = new ObservableCollection<QuestionModel>();
        QuestionsCollection.ItemsSource = Questions;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUncheckedQuestions();
    }

    private async Task LoadUncheckedQuestions()
    {
        var allQuestions = await _firebaseClient
            .Child("roastfriends")
            .Child("question")
            .OnceAsync<QuestionModel>();

        Questions.Clear();
        foreach (var question in allQuestions)
        {
            var questionWithId = question.Object;
            questionWithId.ID = question.Key;
            if (questionWithId.Status == "UNCHECKED")
            {
                Questions.Add(questionWithId);
            }
        }
    }

    private async void Add_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var question = (QuestionModel)button.CommandParameter;

        question.Status = "CHECKED";
        await _firebaseClient
            .Child("roastfriends")
            .Child("question")
            .Child(question.ID)
            .PutAsync(question);


        var userCounter = await _firebaseClient
            .Child("roastfriends")
            .Child("users")
            .Child(question.UserID)
            .Child("counter")
            .OnceSingleAsync<int>();

        userCounter++;

        await _firebaseClient
            .Child("roastfriends")
            .Child("users")
            .Child(question.UserID)
            .Child("counter")
            .PutAsync(userCounter);

        await LoadUncheckedQuestions();
    }

    private async void Delete_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var question = (QuestionModel)button.CommandParameter;

        await _firebaseClient
            .Child("roastfriends")
            .Child("question")
            .Child(question.ID)
            .DeleteAsync();

        await LoadUncheckedQuestions();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//userprofile");
    }
}
