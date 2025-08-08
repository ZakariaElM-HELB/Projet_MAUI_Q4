using MyApp.ViewModel;

namespace MyApp.View;

public partial class StatsView : ContentPage
{
    public StatsView(Member member)
    {
        InitializeComponent();

        if (member == null)
        {
            Shell.Current.DisplayAlert("Erreur", "Aucun membre n'a été sélectionné.", "OK");
            return;
        }

        BindingContext = new GraphViewModel(member);
    }
}
