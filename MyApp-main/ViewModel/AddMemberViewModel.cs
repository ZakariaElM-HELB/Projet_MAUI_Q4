using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using MyApp.Service;
using System;
using System.Linq;
using MyApp.Model;

namespace MyApp.ViewModel
{
    public partial class AddMemberViewModel : BaseViewModel
    {
        private readonly JSONServices _jsonServices;

        [ObservableProperty]
        private string firstName = string.Empty;

        [ObservableProperty]
        private string lastName = string.Empty;

        [ObservableProperty]
        private DateTime birthDate = DateTime.Today;

        [ObservableProperty]
        private string profilePicture = "default_profile.png"; // URL par défaut

        public AddMemberViewModel(JSONServices jsonServices)
        {
            _jsonServices = jsonServices;
        }

        [RelayCommand]
        public async Task SaveNewMember()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var members = await _jsonServices.GetMembers();

                // ✅ Génère le premier ID libre disponible
                int newId = 1;
                var existingIds = members.Select(m => int.TryParse(m.Id, out var id) ? id : -1).ToHashSet();

                while (existingIds.Contains(newId))
                    newId++;

                var newMember = new Member
                {
                    Id = newId.ToString(),
                    FirstName = FirstName,
                    LastName = LastName,
                    BirthDate = BirthDate.ToString("yyyy-MM-dd"),
                    ProfilePicture = ProfilePicture,

                    // ✅ Initialisation des visites hebdomadaires
                    WeeklyVisits = new()
                    {
                        { "Lundi", 0 },
                        { "Mardi", 0 },
                        { "Mercredi", 0 },
                        { "Jeudi", 0 },
                        { "Vendredi", 0 },
                        { "Samedi", 0 },
                        { "Dimanche", 0 }
                    },

                    // ✅ Date du dernier reset
                    LastResetDate = DateTime.Today
                };

                members.Add(newMember);
                Globals.MyMembers = members;
                await _jsonServices.SetMembers();

                Console.WriteLine($"✅ Nouveau membre ajouté : {newMember.FirstName} {newMember.LastName} (ID: {newMember.Id})");

                await Shell.Current.GoToAsync(".."); // 🔙 Retour à la page précédente
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
