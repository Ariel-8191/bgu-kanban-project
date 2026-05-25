using System;
using System.Collections.Generic;
using System.Linq;
using IntroSE.Kanban.Backend.BusinessLayer.Board;

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
                return new Response<BoardSL>(board).ToJson();
            }
            catch (Exception ex)
            {
                return new Response<BoardSL>(ex.Message).ToJson();
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
                return new Response<BoardSL>(board).ToJson();
            }
            catch (Exception ex)
            {
                return new Response<BoardSL>(ex.Message).ToJson();
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
                return new Response<object>().ToJson();
            }
            catch (Exception ex)
            {
                return new Response<object>(ex.Message).ToJson();
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
                return new Response<List<TaskSL>>(inProgressTasksSL).ToJson();
            }
            catch (Exception ex)
            {
                return new Response<List<TaskSL>>(ex.Message).ToJson();
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
                return new Response<List<TaskSL>>(columnTasksSL).ToJson();
            }
            catch (Exception ex)
            {
                return new Response<List<TaskSL>>(ex.Message).ToJson();
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
                return new Response<int?>(limit).ToJson();
            }
            catch (Exception ex)
            {
                return new Response<int?>(ex.Message).ToJson();
            }

        }
    }
}
