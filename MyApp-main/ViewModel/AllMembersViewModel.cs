using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyApp.Service;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace MyApp.ViewModel;

public partial class AllMembersViewModel : BaseViewModel
{
    private readonly JSONServices _jsonServices;
    private readonly CSVServices _csvServices;

    [ObservableProperty]
    private ObservableCollection<Member> members = new();

    [ObservableProperty]
    private string searchQuery;

    [ObservableProperty]
    private string selectedSubscription = "Tous";

    [ObservableProperty]
    private ObservableCollection<Member> filteredMembers = new();

    [ObservableProperty]
    private string sortBy = "ID";

    public ICommand UploadJsonCommand { get; }

    public AllMembersViewModel(JSONServices jsonServices, CSVServices csvServices)
    {
        _jsonServices = jsonServices;
        _csvServices = csvServices;
        UploadJsonCommand = new Command(async () => await UploadJsonToServer());

        _ = LoadMembers();
    }

    private async Task LoadMembers()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var loaded = await _jsonServices.GetMembers();

            Globals.MyMembers = loaded; // ✅ ajout essentiel

            Members = new ObservableCollection<Member>(loaded);
            ApplyFilters();
        }
        finally
        {
            IsBusy = false;
        }
    }


    [RelayCommand]
    public async Task ExportCSVMembers()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            await Shell.Current.Navigation.PushAsync(new SelectColumnsView(Members.ToList()));
        }
        finally
        {
            IsBusy = false;
        }
    }


    [RelayCommand]
    public async Task ImportCSVMembers()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var imported = await _csvServices.LoadData();
            if (imported == null || imported.Count == 0)
            {
                await Shell.Current.DisplayAlert("Import échoué", "Aucun membre importé.", "OK");
                return;
            }

            // ✅ On remplace complètement les membres
            Globals.MyMembers = imported;
            Members = new ObservableCollection<Member>(imported);

            await _jsonServices.SetMembers(); // Sauvegarde locale + upload distant

            await Shell.Current.DisplayAlert("Succès", $"{imported.Count} membres ont été importés (anciens remplacés).", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }


    [RelayCommand]
    public async Task EditMember(Member member)
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            await Shell.Current.Navigation.PushAsync(new EditMemberView(member.Id));
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task ShowStats(Member member)
    {
        if (IsBusy || member == null) return;
        IsBusy = true;

        try
        {
            // ✅ Recherche du vrai membre dans Globals (référence partagée)
            var globalMember = Globals.MyMembers.FirstOrDefault(m => m.Id == member.Id);

            if (globalMember != null)
            {
                await Shell.Current.Navigation.PushAsync(new StatsView(globalMember));
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Membre introuvable dans Globals.MyMembers.", "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }




    [RelayCommand]
    public async Task DeleteMember(Member memberToDelete)
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            if (memberToDelete == null || string.IsNullOrEmpty(memberToDelete.Id))
            {
                await Shell.Current.DisplayAlert("Erreur", "Impossible de supprimer : ID invalide.", "OK");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert(
                "Confirmation",
                $"Supprimer {memberToDelete.FirstName} {memberToDelete.LastName} ?",
                "Oui", "Non");

            if (!confirm)
                return;

            var members = await _jsonServices.GetMembers();
            var target = members.FirstOrDefault(m => m.Id == memberToDelete.Id);

            if (target == null)
            {
                await Shell.Current.DisplayAlert("Erreur", "Membre introuvable dans le JSON.", "OK");
                return;
            }

            members.Remove(target);
            Globals.MyMembers = members;
            await _jsonServices.SetMembers();

            await RefreshPage();
            ApplyFilters();

            await Shell.Current.DisplayAlert("Succès", "Membre supprimé avec succès.", "OK");
            Console.WriteLine($"✅ Membre supprimé : {target.FirstName} {target.LastName}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task RefreshPage()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            Console.WriteLine("🔄 Rafraîchissement des membres depuis le fichier JSON...");

            var members = await _jsonServices.GetMembers();

            if (members == null || members.Count == 0)
            {
                Console.WriteLine("⚠ Aucun membre trouvé lors du rafraîchissement.");
                Members.Clear();
            }
            else
            {
                Members = new ObservableCollection<Member>(members);
                Console.WriteLine($"✅ {Members.Count} membres rechargés avec succès.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur lors du rafraîchissement : {ex.Message}");
            await Shell.Current.DisplayAlert("Erreur", "Impossible de rafraîchir la liste des membres.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Mise à jour automatique du filtrage
    partial void OnSearchQueryChanged(string value)
    {
        ApplyFilters();
    }

    partial void OnSelectedSubscriptionChanged(string value)
    {
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var query = SearchQuery?.ToLower() ?? "";
        var type = SelectedSubscription;

        var filtered = Members.Where(m =>
            (string.IsNullOrEmpty(query) ||
             m.FirstName.ToLower().Contains(query) ||
             m.LastName.ToLower().Contains(query) ||
             m.Id.Contains(query))
            &&
            (type == "Tous" || m.SubscriptionType?.Equals(type, StringComparison.OrdinalIgnoreCase) == true)
        );

        filtered = SortBy switch
        {
            "Nom" => filtered.OrderBy(m => m.LastName).ThenBy(m => m.FirstName),
            "ID" => filtered.OrderBy(m => int.TryParse(m.Id, out var id) ? id : int.MaxValue),
            _ => filtered
        };

        FilteredMembers = new ObservableCollection<Member>(filtered);
    }

    partial void OnSortByChanged(string value)
    {
        ApplyFilters();
    }

    private async Task UploadJsonToServer()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var service = new JSONServices();
            await service.ForceUploadJson();
        }
        finally
        {
            IsBusy = false;
        }
    }
}
