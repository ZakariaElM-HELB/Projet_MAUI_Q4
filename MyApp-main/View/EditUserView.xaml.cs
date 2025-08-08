using MyApp.Model;
using MyApp.ViewModel;

namespace MyApp.View;

public partial class EditUserView : ContentPage
{
    public EditUserView(User selectedUser)
    {
        InitializeComponent();

        var viewModel = new EditUserViewModel();
        BindingContext = viewModel;

        viewModel.InitializeUser(selectedUser);
    }
}
