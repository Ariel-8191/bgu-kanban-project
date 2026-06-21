using System;
using System.Collections.Generic;
using System.Linq;
using IntroSE.Kanban.Backend.BusinessLayer.Board;
using IntroSE.Kanban.Backend.BusinessLayer.CrossCutting;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Provides service-layer functionality for managing Kanban boards within the application.
    /// </summary>
    public class BoardService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private BoardFacade boardFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardService"/> class.
        /// </summary>
        /// <param name="boardFacade">The board facade instance that will handle the core logic.</param>
        internal BoardService(BoardFacade boardFacade)
        {
            this.boardFacade = boardFacade;
        }

        /// <summary>
        /// This method creates a board for the given user.
        /// </summary>
        /// <param name="email">Email of the user, must be logged in</param>
        /// <param name="boardName">The name of the new board</param>
        /// <returns>A JSON representation of the new board</returns>
        public string CreateBoard(string email, string boardName)
        {
            try
            {
                BoardSL board = new BoardSL(this.boardFacade.CreateBoard(email, boardName));
                log.Info($"Successfully created board '{boardName}' for user '{email}'.");
                return new Response<BoardSL>(board).ToJson();
            }
            catch (KanbanException ex)
            {
                return new Response<BoardSL>(ex.Message).ToJson();
            }
            catch (Exception ex)
            {
                log.Error($"An unexpected system error occurred in CreateBoard(email='{email}', board='{boardName}'): {ex.Message}");
                return new Response<BoardSL>("An unexpected system error occurred").ToJson();
            }
        }

        /// <summary>
        /// This method deletes a board.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in and an owner of the board.</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>A JSON representation of the deleted board</returns>
        public string DeleteBoard(string email, string boardName)
        {
            try
            {
                BoardSL board = new BoardSL(this.boardFacade.DeleteBoard(email, boardName));
                log.Info($"Successfully deleted board '{boardName}' for user '{email}'.");
                return new Response<BoardSL>(board).ToJson();
            }
            catch (KanbanException ex)
            {
                return new Response<BoardSL>(ex.Message).ToJson();
            }
            catch (Exception ex)
            {
                log.Error($"An unexpected system error occurred in DeleteBoard(email='{email}', board='{boardName}'): {ex.Message}");
                return new Response<BoardSL>("An unexpected system error occurred").ToJson();
            }
        }

        /// <summary>
        /// This method adds a user as member to an existing board.
        /// </summary>
        /// <param name="email">The email of the user that joins the board. Must be logged in</param>
        /// <param name="boardID">The board's ID</param>
        /// <returns>Returns a JSON representation of the joined board</returns>
        public string JoinBoard(string email, long boardID)
        {
            try
            {
                BoardSL board = new BoardSL(this.boardFacade.JoinBoard(email, boardID));
                log.Info($"Successfully Joined board: '{boardID}' for user '{email}'.");
                return new Response<BoardSL>(board).ToJson();
            }
            catch (KanbanException ex)
            {
                return new Response<BoardSL>(ex.Message).ToJson();
            }
            catch (Exception ex)
            {
                log.Error($"An unexpected system error occurred in JoinBoard(email='{email}', boardID='{boardID}'): {ex.Message}");
                return new Response<BoardSL>("An unexpected system error occurred").ToJson();
            }
        }

        /// <summary>
        /// This method removes a user from the members list of a board.
        /// </summary>
        /// <param name="email">The email of the user. Must be logged in</param>
        /// <param name="boardID">The board's ID</param>
        /// <returns>Returns a JSON representation of the left board</returns>
        public string LeaveBoard(string email, long boardID)
        {
            try
            {
                BoardSL board = new BoardSL(this.boardFacade.LeaveBoard(email, boardID));
                log.Info($"Successfully left board: '{boardID}' for user '{email}'.");
                return new Response<BoardSL>(board).ToJson();
            }
            catch (KanbanException ex)
            {
                return new Response<BoardSL>(ex.Message).ToJson();
            }
            catch (Exception ex)
            {
                log.Error($"An unexpected system error occurred in JoinBoard(email='{email}', boardID='{boardID}'): {ex.Message}");
                return new Response<BoardSL>("An unexpected system error occurred").ToJson();
            }
        }

        /// <summary>
        /// This method transfers a board ownership.
        /// </summary>
        /// <param name="currentOwnerEmail">Email of the current owner. Must be logged in</param>
        /// <param name="newOwnerEmail">Email of the new owner</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>Returns a JSON representation of the transfered board</returns>
        public string TransferOwnership(string currentOwnerEmail, string newOwnerEmail, string boardName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method returns a board's name
        /// </summary>
        /// <param name="boardID">The board's ID</param>
        /// <returns>A response with the board's name</returns>
        public string GetBoardName(long boardID)
        {
            try
            {
                string boardName = this.boardFacade.GetBoardName(boardID);
                log.Info($"Successfully retrieved name of board with ID '{boardID}'.");
                return new Response<string>(null, boardName).ToJson();
            }
            catch (KanbanException ex)
            {
                return new Response<string>(ex.Message).ToJson();
            }
            catch (Exception ex)
            {
                log.Error($"An unexpected system error occurred in GetBoardName(boardID='{boardID}'): {ex.Message}");
                return new Response<string>("An unexpected system error occurred").ToJson();
            }
        }

        /// <summary>
        /// This method returns a list of IDs of all user's boards.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <returns>A response with a list of IDs of all user's boards,</returns>
        public string GetUserBoards(string email)
        {
            try
            {
                List<BoardBL> userBoardsBL = this.boardFacade.GetUserBoards(email);
                List<long> userBoardIDs = userBoardsBL.Select(board => board.BoardID).ToList();
                log.Info($"Successfully retrieved all boards for user '{email}'");
                return new Response<List<long>>(userBoardIDs).ToJson();
            }
            catch (KanbanException ex)
            {
                return new Response<List<BoardSL>>(ex.Message).ToJson();
            }
            catch (Exception ex)
            {
                log.Error($"An unexpected system error occurred in GetUserBoards(email='{email}'): {ex.Message}");
                return new Response<List<BoardSL>>("An unexpected system error occurred").ToJson();
            }
        }

        /// <summary>
        /// This method returns all in-progress tasks of a user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <returns>A response with a list of the in-progress tasks of the user</returns>
        public string GetInProgressTasks(string email)
        {
            try
            {
                List<TaskBL> inProgressTasksBL = this.boardFacade.GetInProgressTasks(email);
                List<TaskSL> inProgressTasksSL = inProgressTasksBL.Select(task => new TaskSL(task)).ToList();
                log.Info($"Successfully retrieved all in-progress tasks of user '{email}'");
                return new Response<List<TaskSL>>(inProgressTasksSL).ToJson();
            }
            catch (KanbanException ex)
            {
                return new Response<List<TaskSL>>(ex.Message).ToJson();
            }
            catch (Exception ex)
            {
                log.Error($"An unexpected system error occurred in GetInProgressTasks(email='{email}'): {ex.Message}");
                return new Response<List<TaskSL>>("An unexpected system error occurred").ToJson();
            }
        }

        /// <summary>
        /// This method limits the number of tasks in a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnIndex">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of null indicates no limit.</param>
        /// <returns>An empty response</returns>
        public string LimitTasksInColumn(string email, string boardName, int columnIndex, int? limit)
        {
            try
            {
                this.boardFacade.LimitTasksInColumn(email, boardName, columnIndex, limit);
                log.Info($"Successfully set task limit to {limit} for column with index {columnIndex} in board '{boardName}' (User: '{email}').");
                return new Response<object>().ToJson();
            }
            catch (KanbanException ex)
            {
                return new Response<object>(ex.Message).ToJson();
            }
            catch (Exception ex)
            {
                log.Error($"An unexpected system error occurred in LimitTasksInColumn(email='{email}', board='{boardName}', column={columnIndex}, limit={limit}): {ex.Message}");
                return new Response<object>("An unexpected system error occurred").ToJson();
            }
        }

        /// <summary>
        /// This method returns all tasks within a specific column of a board for a user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board containing the column</param>
        /// <param name="columnIndex">The index of the column to retrieve tasks from</param>
        /// <returns>A response with a list of the tasks in the specified column</returns>
        public string GetColumnTasks(string email, string boardName, int columnIndex)
        {
            try
            {
                List<TaskBL> columnTasksBL = this.boardFacade.GetColumnTasks(email, boardName, columnIndex);
                List<TaskSL> columnTasksSL = columnTasksBL.Select(task => new TaskSL(task)).ToList();
                log.Info($"Successfully retrieved {columnTasksSL.Count} tasks from column with index {columnIndex} in board '{boardName}' (User: '{email}').");
                return new Response<List<TaskSL>>(columnTasksSL).ToJson();
            }
            catch (KanbanException ex)
            {
                return new Response<List<TaskSL>>(ex.Message).ToJson();
            }
            catch (Exception ex)
            {
                log.Error($"An unexpected system error occurred in GetColumnTasks(email='{email}', board='{boardName}', column={columnIndex}): {ex.Message}");
                return new Response<List<TaskSL>>("An unexpected system error occurred").ToJson();
            }
        }

        /// <summary>
        /// This method returns the column name within a specific board for a user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board containing the column</param>
        /// <param name="columnIndex">The index of the column to retrieve the name of</param>
        /// <returns>A response with a string of the specified column</returns>
        public string GetColumnName(string email, string boardName, int columnIndex)
        {
            try
            {
                string columnName = this.boardFacade.GetColumnName(email, boardName, columnIndex);
                log.Info($"Successfully retrieved name of column with index {columnIndex} in board '{boardName}' (User: '{email}').");
                return new Response<string>(null, columnName).ToJson();
            }
            catch (KanbanException ex)
            {
                return new Response<string>(ex.Message).ToJson();
            }
            catch (Exception ex)
            {
                log.Error($"An unexpected system error occurred in GetColumnName(email='{email}', board='{boardName}', column={columnIndex}): {ex.Message}");
                return new Response<string>("An unexpected system error occurred").ToJson();
            }
        }

        /// <summary>
        /// This method returns the task limit for a specific column of a board for a user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board containing the column</param>
        /// <param name="columnIndex">The index of the column to retrieve the limit for</param>
        /// <returns>A response with the task limit for the specified column, or null if there is no limit</returns>
        public string GetColumnLimit(string email, string boardName, int columnIndex)
        {
            try
            {
                int? limit = this.boardFacade.GetColumnLimit(email, boardName, columnIndex);
                log.Info($"Successfully retrieved task limit of column with index {columnIndex} in board '{boardName}' (User: '{email}').");
                return new Response<int?>(limit).ToJson();
            }
            catch (KanbanException ex)
            {
                return new Response<int?>(ex.Message).ToJson();
            }
            catch (Exception ex)
            {
                log.Error($"An unexpected system error occurred in GetColumnLimit(email='{email}', board='{boardName}', column={columnIndex}): {ex.Message}");
                return new Response<int?>("An unexpected system error occurred").ToJson();
            }
        }
    }
}
