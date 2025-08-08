using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyApp.Service;
using System.Text.Json;
using System.Globalization;

namespace MyApp.ViewModel;

[QueryProperty(nameof(Id), "selectedMember")]
public partial class EditMemberViewModel : BaseViewModel
{
    private readonly JSONServices _jsonServices;

    [ObservableProperty]
    public partial string? Id { get; set; }

    [ObservableProperty] public partial string? FirstName { get; set; }
    [ObservableProperty] public partial string? LastName { get; set; }
    [ObservableProperty] public partial string? BirthDate { get; set; }
    [ObservableProperty] public partial string? ProfilePicture { get; set; }
    [ObservableProperty] public partial string? SubscriptionType { get; set; }
    [ObservableProperty] public partial string? ProfileBackground { get; set; }
    [ObservableProperty] private string? age;

    public EditMemberViewModel(JSONServices jsonServices)
    {
        _jsonServices = jsonServices;
    }

    public async Task LoadMemberAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            if (string.IsNullOrEmpty(Id)) return;

            var members = await _jsonServices.GetMembers();
            var member = members.FirstOrDefault(m => m.Id == Id);

            if (member != null)
            {
                FirstName = member.FirstName;
                LastName = member.LastName;
                BirthDate = member.BirthDate;
                ProfilePicture = member.ProfilePicture;
                SubscriptionType = string.IsNullOrEmpty(member.SubscriptionType) ? "Gratuit" : member.SubscriptionType;
                ProfileBackground = member.SubscriptionType switch
                {
                    "Premium" => "bg_gold.jpg",
                    "Standard" => "bg_silver.jpg",
                    _ => "bg_bronze.jpg"
                };
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SaveChanges()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            if (string.IsNullOrEmpty(Id)) return;

            var members = await _jsonServices.GetMembers();
            var member = members.FirstOrDefault(m => m.Id == Id);

            if (member != null)
            {
                member.FirstName = FirstName ?? string.Empty;
                member.LastName = LastName ?? string.Empty;

                if (DateTime.TryParse(BirthDate, out var parsedDate))
                {
                    member.BirthDate = parsedDate.ToString("yyyy-MM-dd");
                }
                else
                {
                    member.BirthDate = "N/A";
                }

                member.ProfilePicture = ProfilePicture ?? string.Empty;
                member.SubscriptionType = SubscriptionType;

                Globals.MyMembers = members;
                await _jsonServices.SetMembers();

                await Shell.Current.DisplayAlert("Succès", "Les modifications ont été enregistrées dans le JSON.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Impossible de modifier : Membre introuvable.", "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private string CalculateAge(string birthDateString)
    {
        if (string.IsNullOrWhiteSpace(birthDateString)) return "N/A";

        try
        {
            string[] formats = { "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy", "M/d/yyyy h:mm:ss tt" };

            if (DateTime.TryParseExact(birthDateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
            {
                int age = DateTime.Now.Year - birthDate.Year;
                if (DateTime.Now < birthDate.AddYears(age)) age--;
                return age.ToString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur Age : {ex.Message}");
        }

        return "N/A";
    }

    partial void OnBirthDateChanged(string value)
    {
        Age = CalculateAge(value);
    }
}
