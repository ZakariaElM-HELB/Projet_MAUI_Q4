namespace MyApp.View;

public partial class SelectColumnsView : ContentPage
{
    public SelectColumnsView(List<Member> members)
    {
        InitializeComponent();
        BindingContext = new SelectColumnsViewModel(members);
    }
}
