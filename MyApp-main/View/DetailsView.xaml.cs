using MyApp.ViewModel;

namespace MyApp.View
{
    public partial class DetailsView : ContentPage
    {
        private readonly DetailsViewModel _viewModel;

        public DetailsView(DetailsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (string.IsNullOrEmpty(_viewModel.Id))
            {
                await DisplayAlert("Erreur", "Aucun ID reçu pour DetailsView !", "OK");
                return;
            }

            // Utilisation d'une méthode ViewModel pour initialiser le scanner et charger les données
            await _viewModel.InitScannerAndLoadDataAsync();  // 🔄 Appeler la méthode dans le ViewModel
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.CloseScanner();  // Fermeture du scanner proprement via le ViewModel
        }

        private async void OnStatsClicked(object sender, EventArgs e)
        {
            if (_viewModel.Member != null)
            {
                await Navigation.PushAsync(new StatsView(_viewModel.Member));
            }
            else
            {
                await DisplayAlert("Erreur", "Impossible de générer les statistiques : membre non chargé.", "OK");
            }
        }
    }
}
