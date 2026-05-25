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

        private Dictionary<string, Dictionary<string, BoardBL>> boards;
        private AuthenticationFacade authenticationFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardFacade"/> class.
        /// </summary>
        /// <param name="authenticationFacade">The authentication facade used to verify users.</param>
        public BoardFacade(AuthenticationFacade authenticationFacade)
        {
            this.boards = new Dictionary<string, Dictionary<string, BoardBL>>(StringComparer.OrdinalIgnoreCase);
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
                log.Warn("Failed board creation attempt. Reason: Email is null.");
                throw new ArgumentNullException(nameof(email), "Email cannot be null.");
            }
            if (!authenticationFacade.IsLoggedIn(email))
            {
                log.WarnFormat("Failed board creation attempt. Reason: User '{0}' isn't logged in.", email);
                throw new InvalidOperationException("User isn't logged in.");
            }
            if (string.IsNullOrEmpty(boardName))
            {
                log.Warn("Failed board creation attempt. Reason: Invalid board name.");
                throw new InvalidOperationException("Invalid board name.");
            }

            Dictionary<string, BoardBL> userBoards;
            bool userHasBoards = boards.TryGetValue(email, out userBoards);
            if (userHasBoards && userBoards.ContainsKey(boardName))
            {
                log.WarnFormat("Failed board creation attempt. Reason: Board name '{0}' taken.", boardName);
                throw new InvalidOperationException("Board name taken.");
            }

            if (!userHasBoards)
            {
                userBoards = new Dictionary<string, BoardBL>(StringComparer.OrdinalIgnoreCase);
                boards.Add(email, userBoards);
            }

            BoardBL newBoard = new BoardBL(boardName);
            userBoards.Add(boardName, newBoard);
            log.InfoFormat("New board '{0}' for user '{1}' created successfully", boardName, email);
            return newBoard;
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
            boards[email].Remove(boardName); // the call to 'GetBoard' in the previous line ensures the input is valid
            log.InfoFormat("Board '{0}' belonging to user '{1}' deleted successfully", boardName, email);
            return board;
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
            log.InfoFormat("New task with ID '{0}' added to board '{1}' for user '{2}'", newTask.TaskID, boardName, email);
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
        public TaskBL EditTask(string email, string boardName, long taskID, string title, DateTime dueDate, string description)
        {
            BoardBL board = GetBoard(email, boardName);
            TaskBL editedTask = board.EditTask(taskID, title, dueDate, description);
            log.InfoFormat("Task with ID '{0}' in board '{1}' for user '{2}' edited successfully", taskID, boardName, email);
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            log.InfoFormat("Retrieved {0} tasks from column index {1} in board '{2}' for user '{3}'",
                    columnTasks.Count, columnIndex, boardName, email);
            return columnTasks;
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
                log.Warn("Failed in-progress tasks retrieval attempt. Reason: Email is null.");
                throw new ArgumentNullException(nameof(email), "Email cannot be null.");
            }
            if (!authenticationFacade.IsLoggedIn(email))
            {
                log.WarnFormat("Failed in-progress tasks retrieval attempt. Reason: User '{0}' isn't logged in.", email);
                throw new InvalidOperationException("User isn't logged in.");
            }
            

            List<TaskBL> inProgressTasks = new List<TaskBL>();

            Dictionary<string, BoardBL> userBoards;
            if (boards.TryGetValue(email, out userBoards))
            {
                foreach (BoardBL board in userBoards.Values)
                {
                    inProgressTasks.AddRange(board.GetColumnTasks(BoardBL.InProgressColumnIndex));
                }
            }

            log.InfoFormat("Succesfully retrieved all in-progress tasks of user '{0}'", email);
            return inProgressTasks;
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
            log.InfoFormat("Retrieved task limit of column index {0} in board '{1}' for user '{2}'. Limit: {3}",
                    columnIndex, boardName, email, limit.HasValue ? limit.Value.ToString() : "No limit");
            return limit;
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

            log.InfoFormat("Successfully set new limit to column with index {0} in the board '{1}' belonging to user '{2}'", columnIndex, boardName, email);
        }

        /// <summary>
        /// Retrieves a user's board.
        /// </summary>
        /// <param name="email">The email address of the user the board belongs to.</param>
        /// <param name="boardName">The name of the board.</param>
        /// <returns>The <see cref="BoardBL"/> object.</returns>
        private BoardBL GetBoard(string email, string boardName)
        {
            if (email == null)
            {
                log.Warn("Failed board retrieval attempt. Reason: Email is null.");
                throw new ArgumentNullException(nameof(email), "Email cannot be null.");
            }
            if (!authenticationFacade.IsLoggedIn(email))
            {
                log.WarnFormat("Failed board retrieval attempt. Reason: User '{0}' isn't logged in.", email);
                throw new InvalidOperationException("User isn't logged in.");
            }
            if (string.IsNullOrEmpty(boardName))
            {
                log.Warn("Failed board retrieval attempt. Reason: Invalid board name.");
                throw new InvalidOperationException("Invalid board name.");
            }
            Dictionary<string, BoardBL> userBoards;
            bool userHasBoards = boards.TryGetValue(email, out userBoards);
            if (!userHasBoards || !userBoards.ContainsKey(boardName))
            {
                log.WarnFormat("Failed board retrieval attempt. Reason: Board '{0}' doesn't exist.", boardName);
                throw new InvalidOperationException("Board doesn't exist.");
            }

            return userBoards[boardName];
        }
    }
}
