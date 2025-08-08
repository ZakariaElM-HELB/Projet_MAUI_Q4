using MyApp.ViewModel;
using MyApp.Services;

namespace MyApp.View
{
    public partial class LoginView : ContentPage
    {
        public LoginView()
        {
            InitializeComponent();

            var userService = new UserService();
            BindingContext = new LoginViewModel(userService);
        }
    }
}
