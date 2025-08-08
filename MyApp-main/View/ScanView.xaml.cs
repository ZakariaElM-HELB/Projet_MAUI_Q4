

namespace MyApp.View;

public partial class ScanView : ContentPage
{
    private readonly ScanViewModel _viewModel;

    public ScanView()
    {
        InitializeComponent();
        BindingContext = _viewModel = new ScanViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.StartScanner();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopScanner();
    }
}
