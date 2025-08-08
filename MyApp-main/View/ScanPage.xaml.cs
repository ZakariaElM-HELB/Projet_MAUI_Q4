using MyApp.Model;
using MyApp.Service;
using MyApp.ViewModel;

namespace MyApp.View;

public partial class ScanPage : ContentPage
{
    private readonly DeviceOrientationService _scanner;

    public ScanPage()
    {
        InitializeComponent();
        _scanner = new DeviceOrientationService();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            if (!_scanner.IsPortOpen)
            {
                _scanner.OpenPort();
                Console.WriteLine("✅ Port série ouvert dans ScanPage.");
            }

            _scanner.SerialBuffer.Changed += OnSerialDataReception;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur à l'ouverture du port : {ex.Message}");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        try
        {
            _scanner.SerialBuffer.Changed -= OnSerialDataReception;
            _scanner.ClosePort();
            Console.WriteLine("🔒 Port série fermé dans ScanPage.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur à la fermeture du port : {ex.Message}");
        }
    }

    private async void OnSerialDataReception(object sender, EventArgs e)
    {
        if (sender is not DeviceOrientationService.QueueBuffer buffer || buffer.Count == 0)
            return;

        await Task.Delay(100);

        if (buffer.Count > 0)
        {
            string scannedId = buffer.Dequeue()?.ToString()?.Trim();

            if (!string.IsNullOrEmpty(scannedId))
            {
                Console.WriteLine($"📡 ID scanné : {scannedId}");

                var member = await JSONServices.GetMemberByIdAsync(scannedId);
                if (member != null)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        var viewModel = DetailsViewModel.FromMember(member);
                        await Navigation.PushAsync(new DetailsView(viewModel));
                    });
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                        DisplayAlert("Erreur", $"Aucun membre trouvé avec l'ID : {scannedId}", "OK"));
                }
            }
        }
    }
}
