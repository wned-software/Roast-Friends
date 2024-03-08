using Firebase.Auth;
using Firebase.Database;
using Roast_Friends.Other;
using System;
using Microsoft.Maui.Controls;
using Firebase.Database.Query;
using System.Threading.Tasks;
using System.Linq; // Make sure to include this for LINQ queries

namespace Roast_Friends.Views;

public partial class AddQuestion : ContentPage
{
    private FirebaseClient _firebaseClient;
    private FirebaseAuthClient _authClient;

    public AddQuestion(FirebaseClient firebaseClient, FirebaseAuthClient authClient)
    {
        InitializeComponent();
        _firebaseClient = firebaseClient;
        _authClient = authClient;
    }

    private async void AddQuestionButton_Clicked(object sender, EventArgs e)
    {
        var uid = await SecureStorage.GetAsync("user_uid");
        if (string.IsNullOrEmpty(uid))
        {
            await DisplayAlert("Error", "Please log in to add a question.", "OK");
            return;
        }

        string questionContent = QuestionContentEntry.Text;

        if (string.IsNullOrWhiteSpace(questionContent))
        {
            await DisplayAlert("Error", "Question content must be provided.", "OK");
            return;
        }

        // Fetch all questions to find the highest ID
        var questions = await _firebaseClient
            .Child("roastfriends")
            .Child("question")
            .OnceAsync<object>();

        // Extracting question keys and finding the highest number
        var lastQuestionId = questions.Select(q => q.Key.TrimStart('q')).Where(q => int.TryParse(q, out _)).Select(int.Parse).DefaultIfEmpty(0).Max();

        // Incrementing the ID for the new question
        string newQuestionId = $"q{lastQuestionId + 1}";

        await _firebaseClient
            .Child("roastfriends")
            .Child("question")
            .Child(newQuestionId)
            .PutAsync(new
            {
                status = "UNCHECKED",
                userID = uid,
                value = questionContent // No longer combining title and content, just content
            });

        await DisplayAlert("Success", "Your question has been added and awaits approval.", "OK");
        QuestionContentEntry.Text = string.Empty; // Clear the content field after posting
    }
}