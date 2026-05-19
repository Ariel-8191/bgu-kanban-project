using System;
using IntroSE.Kanban.Backend.BusinessLayer.Board;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Provides service-layer functionality for managing tasks within the application.
    /// </summary>
    public class TaskService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);\

        private BoardFacade boardFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskService"/> class.
        /// </summary>
        /// <param name="boardFacade">The board facade instance that will handle the core logic.</param>
        internal TaskService(BoardFacade boardFacade)
        {
            this.boardFacade = boardFacade;
        }

        /// <summary>
        /// This method adds a new task.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <returns>Returns a JSON representation of the added task</returns>
        public string AddTask(string email, string boardName, string title, DateTime dueDate, string description)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method edits task parameters.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="taskID">The ID of the task to edited</param>
        /// <param name="title">New title for the task</param>
        /// <param name="dueDate">New Due date of the task</param>
        /// <param name="description">New description of the task</param>
        /// <returns>Returns a JSON representation of the edited task</returns>
        public string EditTask(string email, string boardName, long taskID, string title, DateTime dueDate, string description)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method advances a task to the next column
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnIndex">The index of the column</param>
        /// <param name="taskID">The ID of the task to advance</param>
        /// <returns>Returns a JSON representation of the advanced task</returns>
        public string AdvanceTask(string email, string boardName, int columnIndex, long taskID)
        {
            throw new NotImplementedException();
        }

    }
}
