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

        public Dictionary<long, TaskDAL> Tasks { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (isPersisted && boardID.HasValue && columnIndex.HasValue)
                {
                    columnController.Update(boardID.Value, columnIndex.Value, "Name", value);
                }
                _name = value;
            }
        }

        private int? _taskLimit;
        public int? TaskLimit
        {
            get => _taskLimit;
            set
            {
                if (isPersisted && boardID.HasValue && columnIndex.HasValue)
                {
                    columnController.Update(boardID.Value, columnIndex.Value, "TaskLimit", value);
                }
                _taskLimit = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDAL"/> class.
        /// </summary>
        /// <param name="boardId">The unique identifier of the board this column belongs to.</param>
        /// <param name="columnIndex">The positional index of the column within the board.</param>
        /// <param name="name">The display name of the column.</param>
        /// <param name="taskLimit">The maximum number of tasks allowed in the column, or null if unrestricted.</param>
        /// <param name="isPersisted">Indicates whether the column data is already saved in the database.</param>
        public ColumnDAL(long? boardId, int? columnIndex, string name, int? taskLimit = null, bool isPersisted = false)
        {
            this.columnController = new ColumnController();
            this.taskController = new TaskController();
            this.isPersisted = isPersisted;

            this.boardID = boardId;
            this.columnIndex = columnIndex;
            this._name = name;
            this._taskLimit = taskLimit;

            this.Tasks = new Dictionary<long, TaskDAL>();
        }

        /// <summary>
        /// Persists the current instance by inserting it into the database using existing boardID and columnIndex.
        /// </summary>
        public void Persist()
        {
            if (!isPersisted && boardID.HasValue && columnIndex.HasValue)
            {
                columnController.Insert(boardID.Value, this);
                isPersisted = true;
            }
        }

        /// <summary>
        /// Persists the current instance by first setting the boardID and columnIndex.
        /// </summary>
        /// <param name="boardID">The ID of the board this column belongs to.</param>
        /// <param name="columnIndex">The positional index of the column.</param>
        public void Persist(long boardID, int columnIndex)
        {
            this.boardID = boardID;
            this.columnIndex = columnIndex;
            Persist();
        }

        /// <summary>
        /// Adds a task to the column's task collection.
        /// </summary>
        /// <param name="task">The task data access object to add.</param>
        public void AddTask(TaskDAL task)
        {
            // Assumes TaskDAL has a TaskID property of type long
            Tasks.Add(task.TaskID, task);
        }

        /// <summary>
        /// Removes a task from the column's task collection.
        /// </summary>
        /// <param name="task">The task data access object to remove.</param>
        public void RemoveTask(TaskDAL task)
        {
            Tasks.Remove(task.TaskID);
        }
    }
}