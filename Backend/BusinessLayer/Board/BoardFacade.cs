using System;
using System.Collections.Generic;
using IntroSE.Kanban.Backend.BusinessLayer.CrossCutting;

namespace IntroSE.Kanban.Backend.BusinessLayer.Board
{
    /// <summary>
    /// Represents the facade for managing board-related operations within the business layer.
    /// </summary>
    internal class BoardFacade
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, Dictionary<string, BoardBL>> boardsByUser;
        private AuthenticationFacade authenticationFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardFacade"/> class.
        /// </summary>
        /// <param name="authenticationFacade">The authentication facade used to verify users.</param>
        public BoardFacade(AuthenticationFacade authenticationFacade)
        {
            this.boardsByUser = new Dictionary<string, Dictionary<string, BoardBL>>(StringComparer.OrdinalIgnoreCase);
            this.authenticationFacade = authenticationFacade;
        }

        /// <summary>
        /// Creates a new board for the specified user.
        /// </summary>
        /// <param name="email">The email address of the user creating the board.</param>
        /// <param name="boardName">The name of the new board.</param>
        /// <returns>The created <see cref="BoardBL"/> object.</returns>
        public BoardBL CreateBoard(string email, string boardName)
        {
            if (email == null)
            {
                string message = $"Cannot create the board '{boardName}' because the given email is null.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }
            if (!authenticationFacade.IsLoggedIn(email))
            {
                string message = $"Cannot create the board '{boardName}' because the user '{email}' is not currently logged in.";
                log.Warn(message);
                throw new KanbanAuthenticationException(message);
            }
            if (string.IsNullOrWhiteSpace(boardName))
            {
                string message = $"Cannot create a board for the user '{email}' because the given board name is null or whitespace.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }

            Dictionary<string, BoardBL> userBoards;
            bool userHasBoards = boardsByUser.TryGetValue(email, out userBoards);
            if (userHasBoards && userBoards.ContainsKey(boardName))
            {
                string message = $"Cannot create a board for the user '{email}' because he already has a board with the name '{boardName}'.";
                log.Warn(message);
                throw new KanbanConflictException(message);
            }

            if (!userHasBoards)
            {
                userBoards = new Dictionary<string, BoardBL>(StringComparer.OrdinalIgnoreCase);
                boardsByUser.Add(email, userBoards);
            }

            BoardBL newBoard = new BoardBL(boardName);
            userBoards.Add(boardName, newBoard);
            return newBoard;
        }

        /// <summary>
        /// Retrieves a user's board. Used as a helper method in the rest of the facade.
        /// </summary>
        /// <param name="email">The email address of the user the board belongs to.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <returns>The <see cref="BoardBL"/> object.</returns>
        private BoardBL GetBoard(string email, string boardName)
        {
            if (email == null)
            {
                string message = $"Cannot get the board '{boardName}' because the given email is null.";
                log.Warn(message);
                throw new KanbanNotFoundException(message);
            }
            if (!authenticationFacade.IsLoggedIn(email))
            {
                string message = $"Cannot get the board '{boardName}' because the user '{email}' is not currently logged in.";
                log.Warn(message);
                throw new KanbanAuthenticationException(message);
            }
            if (string.IsNullOrWhiteSpace(boardName))
            {
                string message = $"Cannot get a board of the user '{email}' because the given board name is null or whitespace.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }
            Dictionary<string, BoardBL> userBoards;
            bool userHasBoards = boardsByUser.TryGetValue(email, out userBoards);
            if (!userHasBoards || !userBoards.ContainsKey(boardName))
            {
                string message = $"Cannot get the board '{boardName}' belonging to the user '{email}' because there is no such board.";
                log.Warn(message);
                throw new KanbanNotFoundException(message);
            }

            return userBoards[boardName];
        }

        /// <summary>
        /// Deletes an existing board for the specified user.
        /// </summary>
        /// <param name="email">The email address of the user deleting the board.</param>
        /// <param name="boardName">The name of the board to delete.</param>
        /// <returns>The deleted <see cref="BoardBL"/> object.</returns>
        public BoardBL DeleteBoard(string email, string boardName)
        {
            BoardBL board = GetBoard(email, boardName);
            boardsByUser[email].Remove(boardName); // the call to 'GetBoard' in the previous line ensures the input is valid

            // If the user doesn't have any boards after the deletion, don't store an empty dictionary
            if (boardsByUser[email].Count == 0)
            {
                boardsByUser.Remove(email);
            }
            return board;
        }

        /// <summary>
        /// Retrieves all in-progress tasks for a specific user across their boards.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>A list of in-progress <see cref="TaskBL"/> objects.</returns>
        public List<TaskBL> GetInProgressTasks(string email)
        {
            if (email == null)
            {
                string message = "Cannot get the \"in progress\" tasks of a user because the given email is null.";
                log.Warn(message);
                throw new KanbanNotFoundException(message);
            }
            if (!authenticationFacade.IsLoggedIn(email))
            {
                string message = $"Cannot get the \"in progress\" tasks of the user '{email}' because he is not currently logged in.";
                log.Warn(message);
                throw new KanbanAuthenticationException(message);
            }


            List<TaskBL> inProgressTasks = new List<TaskBL>();

            Dictionary<string, BoardBL> userBoards;
            if (boardsByUser.TryGetValue(email, out userBoards))
            {
                foreach (BoardBL board in userBoards.Values)
                {
                    inProgressTasks.AddRange(board.GetColumnTasks(BoardBL.InProgressColumnIndex));
                }
            }

            return inProgressTasks;
        }

        /// <summary>
        /// Sets a maximum limit of tasks allowed in a specific column.
        /// </summary>
        /// <param name="email">The email address of the user setting the limit.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnIndex">The index of the column to limit.</param>
        /// <param name="limit">The new limit value. Provide null to indicate no limit.</param>
        public void LimitTasksInColumn(string email, string boardName, int columnIndex, int? limit)
        {
            BoardBL board = GetBoard(email, boardName);
            board.LimitTasksInColumn(columnIndex, limit);
        }

        /// <summary>
        /// Retrieves all tasks located in a specific column.
        /// </summary>
        /// <param name="email">The email address of the user requesting the tasks.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnIndex">The index of the column.</param>
        /// <returns>A list of <see cref="TaskBL"/> objects present in the column.</returns>
        public List<TaskBL> GetColumnTasks(string email, string boardName, int columnIndex)
        {
            BoardBL board = GetBoard(email, boardName);
            List<TaskBL> columnTasks = board.GetColumnTasks(columnIndex);
            return columnTasks;
        }

        /// <summary>
        /// Retrieves the name of a specific column.
        /// </summary>
        /// <param name="email">The email address of the user requesting the column name.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnIndex">The index of the column.</param>
        /// <returns>The name of the column as a string.</returns>
        public string GetColumnName(string email, string boardName, int columnIndex)
        {
            BoardBL board = GetBoard(email, boardName);
            string columnName = board.GetColumnName(columnIndex);
            return columnName;
        }

        /// <summary>
        /// Retrieves the task limit for a specific column.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnIndex">The index of the column.</param>
        /// <returns>The integer limit of the column, or null if no limit is set.</returns>
        public int? GetColumnLimit(string email, string boardName, int columnIndex)
        {
            BoardBL board = GetBoard(email, boardName);
            int? limit = board.GetColumnLimit(columnIndex);
            return limit;
        }

        /// <summary>
        /// Adds a new task to a specific board.
        /// </summary>
        /// <param name="email">The email address of the user adding the task.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="title">The title of the new task.</param>
        /// <param name="dueDate">The due date of the task.</param>
        /// <param name="description">The description of the task.</param>
        /// <returns>The newly created <see cref="TaskBL"/> object.</returns>
        public TaskBL AddTask(string email, string boardName, string title, DateTime dueDate, string description)
        {
            BoardBL board = GetBoard(email, boardName);
            TaskBL newTask = board.AddTask(title, dueDate, description);
            return newTask;
        }

        /// <summary>
        /// Edits the details of an existing task.
        /// </summary>
        /// <param name="email">The email address of the user editing the task.</param>
        /// <param name="boardName">The name of the board containing the task.</param>
        /// <param name="taskID">The unique identifier of the task.</param>
        /// <param name="title">The new title for the task.</param>
        /// <param name="dueDate">The new due date for the task.</param>
        /// <param name="description">The new description for the task.</param>
        /// <returns>The edited <see cref="TaskBL"/> object.</returns>
        public TaskBL EditTask(string email, string boardName, int columnIndex, long taskID, string title, DateTime? dueDate, string description)
        {
            BoardBL board = GetBoard(email, boardName);
            TaskBL editedTask = board.EditTask(columnIndex, taskID, title, dueDate, description);
            return editedTask;
        }

        /// <summary>
        /// Advances a task to the next column on the board.
        /// </summary>
        /// <param name="email">The email address of the user advancing the task.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <param name="columnIndex">The current column index of the task.</param>
        /// <param name="taskID">The unique identifier of the task.</param>
        /// <returns>The advanced <see cref="TaskBL"/> object.</returns>
        public TaskBL AdvanceTask(string email, string boardName, int columnIndex, long taskID)
        {
            BoardBL board = GetBoard(email, boardName);
            TaskBL advancedTask = board.AdvanceTask(columnIndex, taskID);
            return advancedTask;
        }
    }
}
