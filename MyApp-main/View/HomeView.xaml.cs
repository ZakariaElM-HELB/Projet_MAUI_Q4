using MyApp.ViewModel;

namespace MyApp.View
{
    public partial class HomeView : ContentPage
    {
        public HomeView(HomeViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;  // Assigner le ViewModel ici

            if (App.ConnectedUserEmail == null || App.ConnectedUserRole == null)
            {
                // Rediriger immédiatement vers la page de connexion
                Shell.Current.GoToAsync(nameof(LoginView));
                return;
            }

            WelcomeLabel.Text = $"Bienvenue, {App.ConnectedUsername} ({App.ConnectedUserRole})";

            CreateUserButton.IsVisible = App.ConnectedUserRole == "admin";
            UserListButton.IsVisible = App.ConnectedUserRole == "admin";
        }
    }
}
