using MyApp.ViewModel;
using Microcharts.Maui;


namespace MyApp.View;

public partial class StatsPage : ContentPage
{
    public StatsPage(Member member)
    {
        InitializeComponent();

        if (member == null)
        {
            Console.WriteLine("❌ Membre est null !");
            return;
        }

        BindingContext = new GraphViewModel(member);
    }
}


