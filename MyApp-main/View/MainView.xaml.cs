using System.Threading.Tasks;

namespace MyApp.View;

public partial class MainView : ContentPage
{
	MainViewModel viewModel;
	public MainView(MainViewModel viewModel)
	{
		this.viewModel = viewModel;
		InitializeComponent();
		BindingContext=viewModel;
	}
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        BindingContext = null;
        viewModel.RefreshPage();
        BindingContext = viewModel;
        _ = viewModel.LoadJSON();
    }
}