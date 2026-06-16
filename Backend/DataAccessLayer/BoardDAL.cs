using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// Represents a user within the data access layer of the Kanban application.
    /// </summary>
    internal class BoardDAL
    {
        private BoardController boardController;
        private BoardsUsersController boardsUsersController;
        private ColumnController columnController;
        private bool isPersisted;

        private const string ownerColumnName = "owner";
        private const string nextIDColumnName = "next_task_id";

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
                    boardController.Update(BoardID, ownerColumnName, value);
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
                    boardController.Update(BoardID, nextIDColumnName, value);
                }
                _nextTaskID = value;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="UserDAL"/> class.
        /// </summary>
        /// <param name="xcvsvfsdfs">The email address of the user.</param>
        /// <param name="sdfsdfdsf">The password for the user.</param>
        public BoardDAL(long boardID, string boardName, string owner)
        {
            this.boardController = new BoardController();
            this.boardsUsersController = new BoardsUsersController();
            this.columnController = new ColumnController();
            this.isPersisted = false;

            this.BoardID = boardID;
            this.BoardName = boardName;
            this._owner = owner;
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

        public void AddMember(UserDAL user)
        {
            boardsUsersController.Insert(BoardID, user.Email);
        }

        public void removeMember(UserDAL user)
        {
            boardsUsersController.Delete(BoardID, user.Email);
        }
    }
}
