using MyApp.ViewModel;
using MyApp.Services;

namespace MyApp.View
{
    public partial class AddUserView : ContentPage
    {
        public AddUserView()
        {
            InitializeComponent();  // Initialisation des composants
            // Lier le ViewModel et injecter le UserService
            BindingContext = new AddUserViewModel();
        }
    }
}
