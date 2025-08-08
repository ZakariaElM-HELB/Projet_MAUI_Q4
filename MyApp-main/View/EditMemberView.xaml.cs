using MyApp.Service;
using MyApp.ViewModel;

namespace MyApp.View;

public partial class EditMemberView : ContentPage
{
    private readonly EditMemberViewModel _viewModel;

    public EditMemberView(string memberId)
    {
        InitializeComponent();

        var jsonService = new JSONServices();
        _viewModel = new EditMemberViewModel(jsonService)
        {
            Id = memberId
        };

        BindingContext = _viewModel;

        this.Appearing += async (s, e) =>
        {
            await _viewModel.LoadMemberAsync();
        };
    }
}
