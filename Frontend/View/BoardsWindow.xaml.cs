using Frontend.Controllers;
using Frontend.Model;
using Frontend.ViewModel;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for BoardsWindow.xaml. Shows the logged-in user's boards
    /// (name and owner) and lets them create and delete boards, and log out.
    /// </summary>
    public partial class BoardsWindow : Window
    {
        private readonly ControllerFactory controllerFactory;
        private readonly BoardsViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardsWindow"/> class.
        /// </summary>
        /// <param name="controllerFactory">The factory providing the controllers to use.</param>
        /// <param name="user">The logged-in user whose boards are shown.</param>
        public BoardsWindow(ControllerFactory controllerFactory, UserModel user)
        {
            InitializeComponent();
            this.controllerFactory = controllerFactory;
            this.viewModel = new BoardsViewModel(controllerFactory, user);
            this.DataContext = viewModel;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CreateBoard();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BoardModel board)
            {
                MessageBoxResult confirm = MessageBox.Show(
                    $"Delete board \"{board.Name}\"? All of its tasks will be deleted.",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm == MessageBoxResult.Yes)
                {
                    viewModel.DeleteBoard(board);
                }
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.Logout())
            {
                LoginWindow loginWindow = new LoginWindow(controllerFactory);
                loginWindow.Show();
                this.Close();
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaxRestore_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            MaxRestoreIcon.Kind = this.WindowState == WindowState.Maximized
                ? PackIconKind.WindowRestore
                : PackIconKind.WindowMaximize;
        }
    }
}
