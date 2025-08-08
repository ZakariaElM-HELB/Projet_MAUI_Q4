using MyApp.ViewModel;

namespace MyApp.View;

public partial class AddMemberView : ContentPage
{
    public AddMemberView(AddMemberViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
