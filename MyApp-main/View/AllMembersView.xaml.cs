using MyApp.ViewModel;

namespace MyApp.View;

public partial class AllMembersView : ContentPage
{
    public AllMembersView(JSONServices json, CSVServices csv)
    {
        InitializeComponent();
        // Lier le ViewModel avec le BindingContext
        BindingContext = new AllMembersViewModel(json, csv);
    }

    // Lorsque la page apparaît, on appelle RefreshPage dans le ViewModel
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AllMembersViewModel vm)
        {
            // Appel de la méthode RefreshPage du ViewModel pour actualiser les données
            await vm.RefreshPage();
        }
    }
}
