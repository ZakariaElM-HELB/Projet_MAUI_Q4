using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyApp.Model;
using MyApp.Service;
using MyApp.View;
using System;
using System.Threading.Tasks;

namespace MyApp.ViewModel;

public partial class ScanViewModel : BaseViewModel
{
    private readonly DeviceOrientationService _scanner;

    public ScanViewModel()
    {
        _scanner = new DeviceOrientationService();
    }

    public async void StartScanner()
    {
        if (_scanner.IsPortOpen)
            return;

        try
        {
            _scanner.OpenPort();
            _scanner.SerialBuffer.Changed += OnSerialDataReception;
            await Shell.Current.DisplayAlert("Succès", "Port série connecté avec succès.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", $"Erreur à l'ouverture du port : {ex.Message}", "OK");
        }
    }

    public async void StopScanner()
    {
        try
        {
            _scanner.SerialBuffer.Changed -= OnSerialDataReception;
            _scanner.ClosePort();
            await Shell.Current.DisplayAlert("Succès", "Port série déconnecté.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", $"Erreur à la fermeture du port : {ex.Message}", "OK");
        }
    }

    private bool _isProcessing = false;

    private async void OnSerialDataReception(object sender, EventArgs e)
    {
        if (_isProcessing || sender is not DeviceOrientationService.QueueBuffer buffer || buffer.Count == 0)
            return;

        _isProcessing = true;
        IsBusy = true;

        try
        {
            await Task.Delay(100);

            if (buffer.Count > 0)
            {
                string scannedId = buffer.Dequeue()?.ToString()?.Trim();

                if (!string.IsNullOrEmpty(scannedId))
                {
                    var member = await JSONServices.GetMemberByIdAsync(scannedId);
                    if (member != null)
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            var viewModel = DetailsViewModel.FromMember(member);
                            await Shell.Current.GoToAsync(nameof(DetailsView), new Dictionary<string, object>
                            {
                                { "ViewModel", viewModel }
                            });
                        });
                    }
                    else
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                            Shell.Current.DisplayAlert("Erreur", $"Aucun membre trouvé avec l'ID : {scannedId}", "OK"));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
                Shell.Current.DisplayAlert("Erreur", $"Erreur réception : {ex.Message}", "OK"));
        }
        finally
        {
            IsBusy = false;
            _isProcessing = false;
        }
    }
}
