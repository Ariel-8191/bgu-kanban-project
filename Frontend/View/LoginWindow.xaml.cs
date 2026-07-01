using Frontend.Controllers;
using Frontend.Model;
using Frontend.ViewModel;
using System.Windows;

namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml. Handles logging in and registering,
    /// and on success opens the boards window.
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly ControllerFactory controllerFactory;
        private readonly LoginViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWindow"/> class.
        /// </summary>
        /// <param name="controllerFactory">The factory providing the controllers to use.</param>
        public LoginWindow(ControllerFactory controllerFactory)
        {
            InitializeComponent();
            this.controllerFactory = controllerFactory;
            this.viewModel = new LoginViewModel(controllerFactory.UserController);
            this.DataContext = viewModel;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            UserModel? user = viewModel.Login(PasswordBox.Password);
            if (user != null)
            {
                OpenBoards(user);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            UserModel? user = viewModel.Register(PasswordBox.Password);
            if (user != null)
            {
                OpenBoards(user);
            }
        }

        /// <summary>
        /// Opens the boards window for the logged-in user and closes the login window.
        /// </summary>
        /// <param name="user">The logged-in user.</param>
        private void OpenBoards(UserModel user)
        {
            throw new NotImplementedException();
        }
    }
}
