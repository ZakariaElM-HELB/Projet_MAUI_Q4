using MyApp.Model;
using MyApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApp.ViewModel
{
    public partial class EditUserViewModel : BaseViewModel
    {
        [ObservableProperty] private string username;
        [ObservableProperty] private string email;
        [ObservableProperty] private string password;
        [ObservableProperty] private string role;
        [ObservableProperty] private string errorMessage;

        public List<string> RoleOptions { get; } = new() { "admin", "user" };

        public bool IsErrorVisible => !string.IsNullOrWhiteSpace(ErrorMessage);

        partial void OnErrorMessageChanged(string oldValue, string newValue)
        {
            OnPropertyChanged(nameof(IsErrorVisible));
        }

        public EditUserViewModel()
        {
            // Pas de chargement fictif ici
        }

        public void InitializeUser(User user)
        {
            Username = user.Username;
            Email = user.Email;
            Role = user.Role; // important d'initialiser aussi le rôle
        }

        [RelayCommand]
        public async Task SaveUser()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "Champs obligatoires manquants.";
                    return;
                }

                await Shell.Current.DisplayAlert("Succès", "Utilisateur modifié ✅", "OK");
                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
