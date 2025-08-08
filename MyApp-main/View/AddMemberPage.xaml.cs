using MyApp.ViewModel;

namespace MyApp.View;

public partial class AddMemberPage : ContentPage
{
    public AddMemberPage(AddMemberViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
