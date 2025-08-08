using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyApp.Model;
using MyApp.Services;
using MyApp.View;
using System.Collections.ObjectModel;

namespace MyApp.ViewModel;

public partial class UserListViewModel : BaseViewModel
{
    private readonly UserService _userService = new();

    [ObservableProperty]
    private ObservableCollection<User> users;

    public UserListViewModel()
    {
        Users = new ObservableCollection<User>();
        LoadUsersCommand.Execute(null);
    }

    [RelayCommand]
    public async Task LoadUsers()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var list = await _userService.GetAllUsersAsync();

            foreach (var user in list)
            {
                user.CanBeDeleted = user.Id != App.ConnectedUserId && user.Role != "admin";
            }

            Users.Clear();
            foreach (var user in list)
                Users.Add(user);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", "Chargement impossible : " + ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeleteUser(string userId)
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            if (string.IsNullOrEmpty(userId)) return;

            bool confirm = await Shell.Current.DisplayAlert("Confirmation", "Supprimer cet utilisateur ?", "Oui", "Non");
            if (!confirm) return;

            await _userService.DeleteUserAsync(userId);
            await Shell.Current.DisplayAlert("Info", "Utilisateur supprimé", "OK");

            await LoadUsers(); // recharge
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", "Suppression impossible : " + ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task EditUser(User selectedUser)
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            if (selectedUser != null)
            {
                await Shell.Current.Navigation.PushAsync(new EditUserView(selectedUser));
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", "Navigation impossible : " + ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
