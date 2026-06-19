using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// Represents a board within the data access layer of the Kanban application.
    /// </summary>
    internal class BoardDAL
    {
        private BoardController boardController;
        private BoardsUsersController boardsUsersController;
        private ColumnController columnController;
        private bool isPersisted;

        public long BoardID { get; }
        public string BoardName { get; }
        private string _owner;
        public string Owner
        {
            get => _owner;
            set
            {
                if (isPersisted)
                {
                    boardController.UpdateOwner(BoardID, value);
                }
                _owner = value;
            }
        }
        public HashSet<string> Members
        {
            get
            {
                return boardsUsersController.GetBoardMembers(BoardID);
            }
        }

        public List<ColumnDAL> Columns
        {
            get
            {
                return columnController.SelectBoardColumns(BoardID);
            }
        }

        private long _nextTaskID;
        public long NextTaskID
        {
            get => _nextTaskID;
            set
            {
                if (isPersisted)
                {
                    boardController.UpdateNextTaskID(BoardID, value);
                }
                _nextTaskID = value;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BoardDAL"/> class.
        /// </summary>
        /// <param name="boardID">The unique identifier for the board.</param>
        /// <param name="boardName">The display name of the board.</param>
        /// <param name="owner">The email address of the user who owns the board.</param>
        public BoardDAL(long boardID, string boardName, string owner)
        {
            this.boardController = new BoardController();
            this.boardsUsersController = new BoardsUsersController();
            this.columnController = new ColumnController();
            this.isPersisted = false;

            this.BoardID = boardID;
            this.BoardName = boardName;
            this.Owner = owner;
            this.NextTaskID = 0;
        }

        /// <summary>
        /// Persists the current instance by inserting it into the database if it hasn't been saved yet.
        /// </summary>
        public void Persist()
        {
            if (!isPersisted)
            {
                boardController.Insert(this);
                isPersisted = true;
            }
        }

        /// <summary>
        /// Adds a user as a member to this board and updates the database.
        /// </summary>
        /// <param name="user">The <see cref="UserDAL"/> instance representing the user to add.</param>
        public void AddMember(UserDAL user)
        {
            boardsUsersController.Insert(BoardID, user.Email);
        }

        /// <summary>
        /// Removes a user from this board's members and deletes the database.
        /// </summary>
        /// <param name="user">The <see cref="UserDAL"/> instance representing the user to remove.</param>
        public void removeMember(UserDAL user)
        {
            boardsUsersController.Delete(BoardID, user.Email);
        }
    }
}
