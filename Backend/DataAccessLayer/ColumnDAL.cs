using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// Represents a column within the data access layer of the Kanban application.
    /// </summary>
    internal class ColumnDAL
    {
        private ColumnController columnController;
        private TaskController taskController;
        private bool isPersisted;

        private long? boardID;
        private int? columnIndex;

        public List<TaskDAL> Tasks
        {
            get
            {
                return taskController.SelectColumnTasks(boardID.Value, columnIndex.Value);
            }
        }

        public string Name { get; }

        private int? _taskLimit;
        public int? TaskLimit
        {
            get => _taskLimit;
            set
            {
                if (isPersisted)
                {
                    columnController.UpdateTaskLimit(boardID.Value, columnIndex.Value, value);
                }
                _taskLimit = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDAL"/> class.
        /// </summary>
        /// <param name="name">The display name of the column.</param>
        /// <param name="taskLimit">The maximum number of tasks allowed in the column, or null if unrestricted.</param>
        public ColumnDAL(string name, int? taskLimit)
        {
            this.columnController = new ColumnController();
            this.taskController = new TaskController();
            this.isPersisted = false;

            this.Name = name;
            this.TaskLimit = taskLimit;
        }

        /// <summary>
        /// Loads a persisted instance of the <see cref="ColumnDAL"/> class.
        /// </summary>
        /// <param name="name">The display name of the column.</param>
        /// <param name="taskLimit">The maximum number of tasks allowed in the column, or null if unrestricted.</param>
        /// <param name="boardID">The boardId which the column belongs to.</param>
        /// <param name="columnIndex">the index of the column in the specific board.</param>
        internal ColumnDAL(string name, int? taskLimit, long boardID, int columnIndex) : this(name, taskLimit)
        {
            this.boardID = boardID;
            this.columnIndex = columnIndex;
            isPersisted = true;
        }

        /// <summary>
        /// A 'fake' persist that ensures the BL isn't aware of the database implementation.
        /// </summary>
        public void Persist() { }

        /// <summary>
        /// Persists the current instance and sets the boardID and columnIndex.
        /// </summary>
        /// <param name="boardID">The ID of the board this column belongs to.</param>
        /// <param name="columnIndex">The positional index of the column.</param>
        public void Persist(long boardID, int columnIndex)
        {
            if (!isPersisted)
            {
                this.boardID = boardID;
                this.columnIndex = columnIndex;
                columnController.Insert(boardID, columnIndex, this);
                isPersisted = true;
            }
        }

        /// <summary>
        /// Adds a task to the column's task collection.
        /// </summary>
        /// <param name="task">The task data access object to add.</param>
        public void AddTask(TaskDAL task)
        {
            task.Persist(boardID.Value, columnIndex.Value);
        }

        /// <summary>
        /// A 'fake' RemoveTask that ensures the BL isn't aware of the database implementation.
        /// </summary>
        /// <param name="task">The task data access object to remove.</param>
        public void RemoveTask(TaskDAL task) { }
    }
}