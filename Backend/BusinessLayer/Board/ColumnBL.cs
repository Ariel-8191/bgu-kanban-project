using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IntroSE.Kanban.Backend.BusinessLayer.CrossCutting;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer.Board
{
    /// <summary>
    /// Represents a column within a Kanban board that contains and manages tasks.
    /// </summary>
    internal class ColumnBL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal ColumnDAL columnDTO;

        private Dictionary<long, TaskBL> tasks;
        public string Name { get; }
        private int? _taskLimit;
        public int? TaskLimit
        {
            get => _taskLimit;
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

                columnDTO.TaskLimit = value;
                _taskLimit = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnBL"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        public ColumnBL(string name)
        {
            this.columnDTO = new ColumnDAL(name, null);
            this.Name = name;
            this.tasks = new Dictionary<long, TaskBL>();
            this.columnDTO.Persist();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnBL"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        public ColumnBL(ColumnDAL columnDTO)
        {
            this.columnDTO = columnDTO;
            this.Name = columnDTO.Name;
            this.tasks = new Dictionary<long, TaskBL>();
            foreach (TaskDAL taskDTO in columnDTO.Tasks)
            {
                TaskBL taskBL = new TaskBL(taskDTO);
                tasks.Add(taskBL.TaskID, taskBL);
            }
        }


        /// <summary>
        /// Adds a new task to the column.
        /// </summary>
        /// <param name="task">The task object to be added.</param>
        /// <returns>The added <see cref="TaskBL"/> object.</returns>
        public TaskBL AddTask(TaskBL task)
        {
            if (TaskLimit.HasValue && tasks.Count >= TaskLimit.Value)
            {
                string message = $"Cannot add task to column '{Name}' because the task limit ({TaskLimit.Value}) has been reached.";
                log.Warn(message);
                throw new KanbanInvalidStateException(message);
            }

            columnDTO.AddTask(task.taskDTO);
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
            columnDTO.RemoveTask(task.taskDTO);
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

        /// <summary>
        /// Retrieves a task from the column.
        /// </summary>
        /// <param name="taskID">the ID of the task.</param>
        /// <returns>The <see cref="TaskBL"/> object.</returns>
        internal TaskBL GetTask(long taskID)
        {
            return tasks[taskID];
        }

        /// <summary>
        /// Determines whether a task with the specified ID exists in the column.
        /// </summary>
        /// <param name="taskID">The ID of the task to locate.</param>
        /// <returns><c>true</c> if the collection contains a task with the specified ID; otherwise, <c>false</c>.</returns>
        internal bool ContainsTask(long taskID)
        {
            return tasks.ContainsKey(taskID);
        }
    }
}
