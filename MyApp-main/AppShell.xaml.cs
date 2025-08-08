namespace MyApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Pages principales
            Routing.RegisterRoute(nameof(HomeView), typeof(HomeView));
            Routing.RegisterRoute(nameof(ProfileView), typeof(ProfileView));
            Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));

            // Détail et gestion membres
            Routing.RegisterRoute(nameof(AddMemberView), typeof(AddMemberView));
            Routing.RegisterRoute(nameof(AllMembersView), typeof(AllMembersView));
            Routing.RegisterRoute(nameof(EditMemberView), typeof(EditMemberView));
            Routing.RegisterRoute(nameof(DetailsView), typeof(DetailsView));

            // Utilisateurs
            Routing.RegisterRoute(nameof(AddUserView), typeof(AddUserView));
            Routing.RegisterRoute(nameof(EditUserView), typeof(EditUserView));
            Routing.RegisterRoute(nameof(UserListView), typeof(UserListView));

            // Scanner et statistiques
            Routing.RegisterRoute(nameof(ScanView), typeof(ScanView));
            Routing.RegisterRoute(nameof(StatsView), typeof(StatsView));
            Routing.RegisterRoute(nameof(HomeView), typeof(HomeView));
        }
    }
}
