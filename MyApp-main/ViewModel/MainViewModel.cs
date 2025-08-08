using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MyApp.ViewModel;

public partial class MainViewModel(JSONServices MyJSONService) : BaseViewModel
{
    public ObservableCollection<Member> MyObservableList { get; } = [];

    [RelayCommand]
    internal async Task GoToDetails(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            await Shell.Current.DisplayAlert("Erreur", "Aucun ID valide pour ce membre.", "OK");
            return;
        }

        Console.WriteLine($"Navigation vers DetailsView avec ID : {id}");

        await Shell.Current.GoToAsync($"DetailsView?selectedMember={id}");
    }

    [RelayCommand]
    internal async Task SaveJSON()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            await MyJSONService.SetMembers();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    internal async Task LoadJSON()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            Globals.MyMembers = await MyJSONService.GetMembers();

            MyObservableList.Clear();

            foreach (var item in Globals.MyMembers)
            {
                MyObservableList.Add(item);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    internal void RefreshPage()
    {
        MyObservableList.Clear();

        foreach (var item in Globals.MyMembers)
        {
            MyObservableList.Add(item);
        }
    }
}
