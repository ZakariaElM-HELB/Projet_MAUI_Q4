namespace MyApp.View;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Modifier", "Page de modification en cours de développement", "OK");
    }
}
