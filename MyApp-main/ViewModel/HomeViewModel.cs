using MyApp.Service;
using MyApp.Model;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls; // Pour Shell

namespace MyApp.ViewModel
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly JSONServices _jsonServices;

        [ObservableProperty]
        public ObservableCollection<Member> members;

        // Constructeur
        public HomeViewModel(JSONServices jsonServices)
        {
            _jsonServices = jsonServices;
            LoadMembers();
        }

        // Chargement des membres
        private async void LoadMembers()
        {
            await ExecuteCommandAsync(async () =>
            {
                var loadedMembers = await _jsonServices.GetMembers();
                Members = new ObservableCollection<Member>(loadedMembers);
            }, "Erreur lors du chargement des membres");
        }

        // Exécute les commandes avec gestion d'état (IsBusy) et erreurs
        private async Task ExecuteCommandAsync(Func<Task> action, string errorMessage)
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                await action();
            }
            catch (Exception ex)
            {
                await HandleError(ex, errorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Gestion des erreurs
        private async Task HandleError(Exception ex, string errorMessage)
        {
            await Shell.Current.DisplayAlert("Erreur", $"{errorMessage}: {ex.Message}", "OK");
        }

        // Commande pour scanner un code-barres
        [RelayCommand]
        public async Task ScanBarcode()
        {
            await ExecuteCommandAsync(async () => await Shell.Current.GoToAsync(nameof(ScanView)), "Impossible de scanner");
        }

        // Commande pour voir le profil
        [RelayCommand]
        public async Task ViewProfile()
        {
            await ExecuteCommandAsync(async () => await Shell.Current.GoToAsync(nameof(ProfileView)), "Impossible de voir le profil");
        }

        // Commande pour ajouter un utilisateur
        [RelayCommand]
        public async Task GoToAddUserView()
        {
            await ExecuteCommandAsync(async () => await Shell.Current.GoToAsync(nameof(AddUserView)), "Impossible d'ajouter un utilisateur");
        }

        // Commande pour afficher la liste des utilisateurs
        [RelayCommand]
        public async Task GoToUserListView()
        {
            await ExecuteCommandAsync(async () => await Shell.Current.GoToAsync(nameof(UserListView)), "Impossible d'afficher la liste des utilisateurs");
        }

        // Commande de déconnexion
        [RelayCommand]
        public async Task Logout()
        {
            await ExecuteCommandAsync(async () =>
            {
                App.ConnectedUserEmail = null;
                App.ConnectedUserRole = null;
                App.ConnectedUsername = null;
                App.ConnectedUserId = null;

                Preferences.Clear(); // <--- AJOUTER cette ligne pour bien vider la session sauvegardée

                await Shell.Current.GoToAsync("///LoginView");
            }, "Impossible de se déconnecter");
        }


        // Ouvrir le menu utilisateur
        [RelayCommand]
        public async Task OpenUserMenu()
        {
            string action = await Shell.Current.DisplayActionSheet("Mon compte", "Annuler", null,
                "👤 Voir mon Profil", "🚪 Déconnexion");

            switch (action)
            {
                case "👤 Voir mon Profil":
                    await Shell.Current.GoToAsync(nameof(ProfileView));
                    break;
                case "🚪 Déconnexion":
                    await Logout(); // Utiliser la méthode de déconnexion centralisée
                    break;
            }
        }

        // Commande pour ajouter un membre
        [RelayCommand]
        public async Task GoToAddMemberView()
        {
            await Shell.Current.GoToAsync(nameof(AddMemberView));
        }

        // Commande pour voir tous les membres
        [RelayCommand]
        public async Task GoToAllMembersView()
        {
            await Shell.Current.GoToAsync(nameof(AllMembersView));
        }
    }
}
