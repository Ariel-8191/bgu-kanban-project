using Frontend.Controllers;
using Frontend.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Frontend.ViewModel
{
    /// <summary>
    /// The view model backing the boards window, where a user views, creates,
    /// and deletes their boards.
    /// </summary>
    public class BoardsViewModel : Notifiable
    {
        private readonly BoardController boardController;
        private readonly UserController userController;

        /// <summary>
        /// The currently logged-in user.
        /// </summary>
        public UserModel User { get; }

        /// <summary>
        /// A greeting shown at the top of the window.
        /// </summary>
        public string WelcomeMessage => $"Welcome, {User.Email}!";

        /// <summary>
        /// The boards the user is a member of, bound to the boards list in the view.
        /// </summary>
        public ObservableCollection<BoardModel> Boards { get; }

        private string newBoardName;
        /// <summary>
        /// The name typed by the user when creating a new board.
        /// </summary>
        public string NewBoardName
        {
            get => newBoardName;
            set { newBoardName = value; RaisePropertyChanged(); }
        }

        private BoardModel? selectedBoard;
        /// <summary>
        /// The board currently selected in the list, or <c>null</c> if none is selected.
        /// </summary>
        public BoardModel? SelectedBoard
        {
            get => selectedBoard;
            set { selectedBoard = value; RaisePropertyChanged(); }
        }

        private string errorMessage;
        /// <summary>
        /// The error message shown to the user, empty when there is no error.
        /// </summary>
        public string ErrorMessage
        {
            get => errorMessage;
            set { errorMessage = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardsViewModel"/> class and
        /// loads the user's boards.
        /// </summary>
        /// <param name="controllerFactory">The factory providing the controllers to use.</param>
        /// <param name="user">The logged-in user whose boards are shown.</param>
        public BoardsViewModel(ControllerFactory controllerFactory, UserModel user)
        {
            this.boardController = controllerFactory.BoardController;
            this.userController = controllerFactory.UserController;
            this.User = user;
            this.Boards = new ObservableCollection<BoardModel>();
            this.newBoardName = string.Empty;
            this.errorMessage = string.Empty;
            LoadBoards();
        }

        /// <summary>
        /// Reloads the user's boards from the backend into the <see cref="Boards"/> collection.
        /// </summary>
        public void LoadBoards()
        {
            ErrorMessage = string.Empty;
            try
            {
                List<BoardModel> boards = boardController.GetUserBoards(User.Email);
                Boards.Clear();
                foreach (BoardModel board in boards)
                {
                    Boards.Add(board);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Creates a new board with the name in <see cref="NewBoardName"/> and refreshes the list.
        /// </summary>
        public void CreateBoard()
        {
            ErrorMessage = string.Empty;
            try
            {
                boardController.CreateBoard(User.Email, NewBoardName);
                NewBoardName = string.Empty;
                LoadBoards();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Deletes the given board and refreshes the list.
        /// </summary>
        /// <param name="board">The board to delete.</param>
        public void DeleteBoard(BoardModel board)
        {
            ErrorMessage = string.Empty;
            try
            {
                boardController.DeleteBoard(User.Email, board.Name);
                LoadBoards();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Logs the current user out.
        /// </summary>
        /// <returns><c>true</c> if the logout succeeded, <c>false</c> otherwise.</returns>
        public bool Logout()
        {
            ErrorMessage = string.Empty;
            try
            {
                userController.Logout(User.Email);
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }
    }
}
