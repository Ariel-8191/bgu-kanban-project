using System.Collections.Generic;
using System.Linq;
using IntroSE.Kanban.Backend.BusinessLayer.CrossCutting;

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
                    string message = $"Cannot set new task limit in column '{Name}' because the given value is negative.";
                    log.Warn(message);
                    throw new KanbanValidationException(message);
                }
                if (tasks.Count > value)
                {
                    string message = $"Cannot set new task limit in column '{Name}' because the given limit ({value}) is lower than current task count ({tasks.Count}).";
                    log.Warn(message);
                    throw new KanbanValidationException(message);
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
                string message = $"Cannot add task to column '{Name}' because the task limit ({TaskLimit.Value}) has been reached.";
                log.Warn(message);
                throw new KanbanInvalidStateException(message);
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
