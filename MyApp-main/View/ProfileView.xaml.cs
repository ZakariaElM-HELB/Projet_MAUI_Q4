namespace MyApp.View;

public partial class ProfileView : ContentPage
{
    public ProfileView()
    {
        InitializeComponent();

        // 💡 Affecte les valeurs de l'utilisateur connecté
        NameLabel.Text = App.ConnectedUsername != null ? App.ConnectedUsername : "Non défini";
        EmailLabel.Text = App.ConnectedUserEmail != null ? App.ConnectedUserEmail : "Non défini";
        RoleLabel.Text = App.ConnectedUserRole != null ? App.ConnectedUserRole : "Non défini";

        // ✅ Affiche le bouton modifier uniquement pour admin
        EditButton.IsVisible = App.ConnectedUserRole == "admin";
    }

    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(EditUserView));
    }
}