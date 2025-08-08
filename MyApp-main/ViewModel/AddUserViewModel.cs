using MyApp.Services;
using MyApp.Model;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace MyApp.ViewModel
{
    public partial class AddUserViewModel : BaseViewModel
    {
        private readonly UserService _userService = new UserService();

        // Propriétés observables
        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string role;

        [ObservableProperty]
        private string statusMessage;

        [ObservableProperty]
        private bool isStatusVisible;

        [ObservableProperty]
        private bool isAdmin;

        // Constructeur
        public AddUserViewModel()
        {
            IsAdmin = App.ConnectedUserRole == "admin";
        }

        [RelayCommand]
        public async Task AddUserAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                if (string.IsNullOrWhiteSpace(Username) ||
                    string.IsNullOrWhiteSpace(Email) ||
                    string.IsNullOrWhiteSpace(Password) ||
                    string.IsNullOrWhiteSpace(Role))
                {
                    StatusMessage = "Veuillez remplir tous les champs.";
                    IsStatusVisible = true;
                    return;
                }

                var existingUser = await _userService.GetUserByEmailAsync(Email);
                if (existingUser != null)
                {
                    StatusMessage = "Cet email est déjà utilisé.";
                    IsStatusVisible = true;
                    return;
                }

                var newUser = new User
                {
                    Username = Username,
                    Email = Email,
                    Password = Password,
                    Role = Role
                };

                await _userService.AddUserAsync(newUser);
                StatusMessage = "Utilisateur ajouté avec succès !";
                IsStatusVisible = true;

                // Réinitialisation des champs
                Username = string.Empty;
                Email = string.Empty;
                Password = string.Empty;
                Role = string.Empty;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
