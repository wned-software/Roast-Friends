namespace Roast_Friends.Views;

public partial class UnlockForFree : ContentPage
{
    public UnlockForFree()
	{
		InitializeComponent();
        var current = Connectivity.NetworkAccess;

        if (current == NetworkAccess.Internet)
        {
            // polaczenie aktywne xD
        }
    }
}