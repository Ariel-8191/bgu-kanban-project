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
        /// <param name="name">The name of the new board</param>
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
        /// <param name="name">The name of the board</param>
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
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <returns>An empty response</returns>
        public string LimitTasksInColumn(string email, string boardName, int columnIndex, int? limit)
        {
            throw new NotImplementedException();
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
    }
}
