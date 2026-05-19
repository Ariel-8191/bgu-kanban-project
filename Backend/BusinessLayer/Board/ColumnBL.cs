using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.BusinessLayer.Board
{
    /// <summary>
    /// Represents a column within a Kanban board that contains and manages tasks.
    /// </summary>
    internal class ColumnBL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<long, TaskBL> tasks;
        public string Name { get; }
        public int? _taskLimit;
        public int? TaskLimit
        {
            get { return _taskLimit; }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnBL"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        public ColumnBL(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a new task to the column.
        /// </summary>
        /// <param name="task">The task object to be added.</param>
        /// <returns>The added <see cref="TaskBL"/> object.</returns>
        public TaskBL AddTask (TaskBL task)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes an existing task from the column.
        /// </summary>
        /// <param name="task">The task object to be removed.</param>
        /// <returns>The removed <see cref="TaskBL"/> object.</returns>
        public TaskBL RemoveTask(TaskBL task)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a list of all tasks currently in the column.
        /// </summary>
        /// <returns>A <see cref="List{TaskBL}"/> containing all tasks in the column.</returns>
        public List<TaskBL> GetTasks()
        {
            throw new NotImplementedException();
        }
    }
}
