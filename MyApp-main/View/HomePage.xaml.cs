namespace MyApp.View;

using MyApp.View;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnScanBarcodeClicked(object sender, EventArgs e)
    {
        // Simule une lecture du scanner (remplace ça par ton vrai scan plus tard)
        string scannedID = await SimulateScanAsync();

        if (!string.IsNullOrEmpty(scannedID))
        {
            // Naviguer vers la page DetailsView avec l'ID scanné
            await Navigation.PushAsync(new ScanPage());
        }
        else
        {
            await DisplayAlert("Erreur", "Impossible de lire le code-barres", "OK");
        }
    }

    // Simule un scan (remplace par ton vrai scanner plus tard)
    private Task<string> SimulateScanAsync()
    {
        return Task.FromResult("1"); // Simule le scan du code-barres avec ID = 1
    }


    private async void OnViewProfileClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ProfilePage));
    }

    private async void OnShowAllMembersClicked(object sender, EventArgs e)
    {
        if (BindingContext is HomeViewModel vm)
        {
            await vm.LoadMembersFromJsonAsync();
        }
    }


}

