using Frontend.Controllers;
using Frontend.View;
using System.Windows;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for App.xaml. On startup it loads the persisted data and
    /// opens the login window.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ControllerFactory controllerFactory = new ControllerFactory();

            try
            {
                controllerFactory.LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load saved data: {ex.Message}",
                    "Kanban", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            LoginWindow loginWindow = new LoginWindow(controllerFactory);
            loginWindow.Show();
        }
    }
}
