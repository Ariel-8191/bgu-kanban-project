using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.BusinessLayer.Board
{
    /// <summary>
    /// Represents a Kanban board in the business layer, managing its columns, tasks, and properties.
    /// </summary>
    internal class BoardBL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string BacklogColumnName = "backlog";
        private const string InProgressColumnName = "in progress";
        private const string DoneColumnName = "done";

        public string BoardName { get; }
        private Dictionary<long, TaskBL> tasks;
        private List<ColumnBL> columns;
        private long nextTaskID;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardBL"/> class with the specified board name.
        /// </summary>
        /// <param name="boardName">The name to be assigned to the new board.</param>
        public BoardBL(string boardName)
        {
            this.BoardName = boardName;
            this.tasks = new Dictionary<long, TaskBL>();
            this.nextTaskID = 0;

            this.columns = new List<ColumnBL>();
            columns.Add(new ColumnBL(BacklogColumnName));
            columns.Add(new ColumnBL(InProgressColumnName));
            columns.Add(new ColumnBL(DoneColumnName));
        }

        /// <summary>
        /// Creates and adds a new task to the board. 
        /// </summary>
        /// <param name="title">The title of the new task.</param>
        /// <param name="dueDate">The deadline/due date for the task.</param>
        /// <param name="description">A detailed description of the task.</param>
        /// <returns>The newly created <see cref="TaskBL"/> object.</returns>
        public TaskBL AddTask(string title, DateTime dueDate, string description)
        {
            TaskBL newTask = new TaskBL(this.nextTaskID, title, dueDate, description);
            nextTaskID++;
            this.tasks.Add(newTask.TaskID, newTask);

            ColumnBL backlog = columns[0];
            backlog.AddTask(newTask);

            return newTask;
        }

        /// <summary>
        /// Edits the details of an existing task on the board.
        /// </summary>
        /// <param name="taskID">The unique identifier of the task to edit.</param>
        /// <param name="title">The new title for the task.</param>
        /// <param name="dueDate">The new due date for the task.</param>
        /// <param name="description">The new description for the task.</param>
        /// <returns>The updated <see cref="TaskBL"/> object.</returns>
        public TaskBL EditTask(long taskID, string title, DateTime dueDate, string description)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Advances a specific task from its current column to the next column on the board.
        /// </summary>
        /// <param name="columnIndex">The index of the column where the task is currently located.</param>
        /// <param name="taskID">The unique identifier of the task to advance.</param>
        /// <returns>The updated <see cref="TaskBL"/> object reflecting its new state/column.</returns>
        public TaskBL AdvanceTask(int columnIndex, long taskID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the name of a specific column on the board.
        /// </summary>
        /// <param name="columnIndex">The zero-based index of the column.</param>
        /// <returns>The name of the column as a string.</returns>
        public string GetColumnName(int columnIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a list of all tasks currently located in a specific column.
        /// </summary>
        /// <param name="columnIndex">The zero-based index of the column.</param>
        /// <returns>A list of <see cref="TaskBL"/> objects present in the specified column.</returns>
        public List<TaskBL> GetColumnTasks(int columnIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves all tasks on the board that are currently marked as "in progress".
        /// </summary>
        /// <returns>A list of <see cref="TaskBL"/> objects that are in progress.</returns>
        public List<TaskBL> GetInProgressTasks()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the maximum number of tasks allowed in a specific column.
        /// </summary>
        /// <param name="columnIndex">The zero-based index of the column to query.</param>
        /// <returns>An integer representing the task limit, or null if there is no limit set.</returns>
        public int? getColumnLimit(int columnIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets or removes the maximum limit on the number of tasks allowed in a specific column.
        /// </summary>
        /// <param name="columnIndex">The zero-based index of the column to modify.</param>
        /// <param name="limit">The maximum number of tasks allowed, or null to remove the limit.</param>
        public void LimitTasksInColumn(int columnIndex, int? limit)
        {
            throw new NotImplementedException();
        }
    }
}
