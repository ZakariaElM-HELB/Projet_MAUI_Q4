using MyApp.Model;
using MyApp.Services;

namespace MyApp.View
{
    public partial class UserListView : ContentPage
    {

        public UserListView()
        {
            InitializeComponent();

            BindingContext = new UserListViewModel();
        }

    }
}
