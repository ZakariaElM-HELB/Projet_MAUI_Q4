using MyApp.ViewModel;

namespace MyApp.View;

public partial class AllMembersPage : ContentPage
{
    public AllMembersPage(JSONServices json, CSVServices csv)
    {
        InitializeComponent();
        BindingContext = new AllMembersViewModel(json, csv);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AllMembersViewModel vm)
        {
            await vm.RefreshPage();
        }
    }
}
