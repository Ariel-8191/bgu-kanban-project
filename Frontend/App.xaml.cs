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
        }
    }
}
