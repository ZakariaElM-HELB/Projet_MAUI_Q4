using System.Windows.Input;
using MyApp.Services;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace MyApp.ViewModel
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        private bool keepConnected;

        public ICommand LoginCommand { get; }

        public LoginViewModel(UserService userService)
        {
            _userService = userService;
            LoginCommand = new AsyncRelayCommand(LoginAsync);
        }

        private async Task LoginAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                if (string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "Veuillez entrer votre email.";
                    await Shell.Current.DisplayAlert("Erreur", "Email vide", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Veuillez entrer votre mot de passe.";
                    await Shell.Current.DisplayAlert("Erreur", "Mot de passe vide", "OK");
                    return;
                }

                var user = await _userService.AuthenticateAsync(Email, Password);

                if (user == null)
                {
                    ErrorMessage = "Email ou mot de passe incorrect.";
                    await Shell.Current.DisplayAlert("Erreur", "Email ou mot de passe incorrect.", "OK");
                    return;
                }

                // Connexion réussie
                ErrorMessage = string.Empty;

                App.ConnectedUserEmail = user.Email;
                App.ConnectedUserRole = user.Role;
                App.ConnectedUsername = user.Username;
                App.ConnectedUserId = user.Id;
                App.ConnectedUserCreatedAt = user.CreatedAt;

                Preferences.Set("UserEmail", user.Email);
                Preferences.Set("UserRole", user.Role);
                Preferences.Set("Username", user.Username);
                Preferences.Set("UserId", user.Id);
                Preferences.Set("UserCreatedAt", user.CreatedAt.ToString("O"));
                Preferences.Set("KeepConnected", KeepConnected);

                await Shell.Current.GoToAsync(nameof(HomeView));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erreur", $"Une erreur est survenue : {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
