using System;
using System.Collections.Generic;
using System.Linq;

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
                if (value < 0)
                {
                    log.WarnFormat("Failed setting task limit in column '{0}'. Reason: new limit is negative.", Name);
                    throw new InvalidOperationException("New limit is negative.");
                }
                if (tasks.Count > value)
                {
                    log.WarnFormat("Failed setting task limit in column '{0}'. Reason: new limit ({1}) is lower than current task count ({2}).", Name, value, tasks.Count);
                    throw new InvalidOperationException("New limit is lower than current task count.");
                }

                _taskLimit = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnBL"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        public ColumnBL(string name)
        {
            this.Name = name;
            this.tasks = new Dictionary<long, TaskBL>();
        }

        /// <summary>
        /// Adds a new task to the column.
        /// </summary>
        /// <param name="task">The task object to be added.</param>
        /// <returns>The added <see cref="TaskBL"/> object.</returns>
        public TaskBL AddTask (TaskBL task)
        {
            if (TaskLimit.HasValue && tasks.Count >= TaskLimit.Value)
            {
                log.WarnFormat("Failed adding task to column '{0}'. Reason: task limit of {1} reached.", Name, TaskLimit.Value);
                throw new InvalidOperationException("Cannot add task: column has reached its task limit.");
            }

            tasks.Add(task.TaskID, task);
            return task;
        }

        /// <summary>
        /// Removes an existing task from the column.
        /// </summary>
        /// <param name="task">The task object to be removed.</param>
        /// <returns>The removed <see cref="TaskBL"/> object.</returns>
        public TaskBL RemoveTask(TaskBL task)
        {        
            tasks.Remove(task.TaskID);
            return task;
        }

        /// <summary>
        /// Retrieves a list of all tasks currently in the column.
        /// </summary>
        /// <returns>A <see cref="List{TaskBL}"/> containing all tasks in the column.</returns>
        public List<TaskBL> GetTasks()
        {
            return tasks.Values.ToList();
        }
    }
}
