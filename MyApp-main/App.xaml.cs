namespace MyApp;

public partial class App : Application
{
    public static string ConnectedUserId { get; set; }
    public static string ConnectedUsername { get; set; }
    public static string ConnectedUserEmail { get; set; }
    public static string ConnectedUserRole { get; set; }
    public static DateTime ConnectedUserCreatedAt { get; set; }

    public App()
    {
        InitializeComponent();

        if (Preferences.Get("KeepConnected", false))
        {
            ConnectedUserEmail = Preferences.Get("UserEmail", null);
            ConnectedUserRole = Preferences.Get("UserRole", null);
            ConnectedUsername = Preferences.Get("Username", null);
            ConnectedUserId = Preferences.Get("UserId", null);

            string dateStr = Preferences.Get("UserCreatedAt", null);
            if (DateTime.TryParse(dateStr, out var createdDate))
                ConnectedUserCreatedAt = createdDate;

            MainPage = new AppShell();

            // important : navigation async doit se faire après que le Shell est chargé
            Application.Current.Dispatcher.Dispatch(async () =>
            {
                await Shell.Current.GoToAsync("/HomeView");

            });
        }
        else
        {
            // Ceci garantit que l'app démarre correctement même si pas connecté
            MainPage = new AppShell(); // Shell montrera LoginView par défaut
        }
    }
}
