using MyApp.Services;
using MyApp.Model;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.ViewModel
{
    public partial class DetailsViewModel : BaseViewModel
    {
        private readonly JSONServices _jsonServices;
        private readonly DeviceOrientationService _scannerService;


        [ObservableProperty]
        public partial string? Id { get; set; }

        [ObservableProperty] public partial string? FirstName { get; set; }
        [ObservableProperty] public partial string? LastName { get; set; }
        [ObservableProperty] public partial string? BirthDate { get; set; }
        [ObservableProperty] public partial string? ProfilePicture { get; set; }
        [ObservableProperty] public partial string? SerialBufferContent { get; set; }
        [ObservableProperty] public partial string? SubscriptionType { get; set; }
        [ObservableProperty] public partial string? ProfileBackground { get; set; }

        public Member Member { get; set; }

        public DetailsViewModel(JSONServices jsonServices, DeviceOrientationService scannerService)
        {
            _jsonServices = jsonServices;
            _scannerService = scannerService;
        }

        // Nouvelle méthode combinée pour initialiser le scanner et charger les données du membre
        public async Task InitScannerAndLoadDataAsync()
        {
            try
            {
                // Initialisation du scanner
                if (!_scannerService.IsPortOpen)
                {
                    _scannerService.OpenPort();
                    await Shell.Current.DisplayAlert("Connexion réussie", "Scanner connecté avec succès.", "OK");

                }

                _scannerService.SerialBuffer.Changed -= OnSerialDataReception;
                _scannerService.SerialBuffer.Changed += OnSerialDataReception;

                // Chargement des données du membre
                await RefreshPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur d'initialisation du scanner ou de chargement des données : {ex.Message}");
                await Shell.Current.DisplayAlert("Erreur", "Problème avec l'initialisation du scanner ou des données.", "OK");
            }
        }

        public void CloseScanner()
        {
            try
            {
                if (_scannerService.IsPortOpen)
                {
                    _scannerService.SerialBuffer.Changed -= OnSerialDataReception;
                    _scannerService.ClosePort();
                    Console.WriteLine("✅ Scanner fermé.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur de fermeture du scanner : {ex.Message}");
                Shell.Current.DisplayAlert("Erreur", "Impossible de fermer le scanner.", "OK");
            }
        }

        private async void OnSerialDataReception(object sender, EventArgs arg)
        {
            if (sender is not DeviceOrientationService.QueueBuffer buffer || buffer.Count == 0)
                return;

            await Task.Delay(100);

            if (buffer.Count > 0)
            {
                string scannedId = buffer.Dequeue().ToString()?.Trim();

                if (!string.IsNullOrEmpty(scannedId))
                {
                    SerialBufferContent = $"📡 ID Scanné : {scannedId}";
                    await LoadMemberFromJsonAsync(scannedId);
                }
            }
        }

        private async Task LoadMemberFromJsonAsync(string scannedId)
        {
            var members = await _jsonServices.GetMembers();
            var member = members.FirstOrDefault(m => m.Id == scannedId);

            if (member != null)
            {
                Member = member;
                Id = member.Id;
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
            else
            {
                SerialBufferContent += "\n❌ Aucun membre trouvé.";
            }
        }

        internal async Task RefreshPage()
        {
            if (string.IsNullOrEmpty(Id)) return;

            var members = await _jsonServices.GetMembers();
            var member = members.FirstOrDefault(m => m.Id == Id);

            if (member == null)
            {
                FirstName = "Inconnu";
                LastName = "Membre";
                BirthDate = "N/A";
                ProfilePicture = "default_profile.png";
                ProfileBackground = "bg_bronze.jpg";
                return;
            }

            Member = member;
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

        [RelayCommand]
        internal async Task ChangeObjectParameters()
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
                    member.BirthDate = BirthDate ?? "N/A";
                    member.ProfilePicture = ProfilePicture ?? string.Empty;

                    Globals.MyMembers = members;
                    await _jsonServices.SetMembers();

                    await Shell.Current.DisplayAlert("Succès", "Les modifications ont été enregistrées.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erreur", "Membre introuvable.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task DeleteMember()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                if (string.IsNullOrEmpty(Id)) return;

                var members = await _jsonServices.GetMembers();
                var toDelete = members.FirstOrDefault(m => m.Id == Id);

                if (toDelete != null)
                {
                    members.Remove(toDelete);

                    Globals.MyMembers = members;
                    await _jsonServices.SetMembers();

                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erreur", "Membre introuvable.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task UpdateMember()
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
                    member.FirstName = FirstName;
                    member.LastName = LastName;
                    member.BirthDate = BirthDate;
                    member.ProfilePicture = ProfilePicture;

                    await _jsonServices.SetMembers();

                    await Shell.Current.DisplayAlert("Succès", "Les modifications ont été enregistrées.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erreur", "Le membre n'a pas été trouvé.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static DetailsViewModel FromMember(Member member)
        {
            var vm = new DetailsViewModel(new JSONServices(), new DeviceOrientationService())
            {
                Member = member,
                Id = member.Id,
                FirstName = member.FirstName,
                LastName = member.LastName,
                BirthDate = member.BirthDate,
                ProfilePicture = member.ProfilePicture,
                SubscriptionType = member.SubscriptionType
            };

            return vm;
        }
    }
}
