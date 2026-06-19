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
                _name = value;
                if (isPersisted && boardID.HasValue && columnIndex.HasValue)
                {
                    columnController.Update(boardID.Value, columnIndex.Value, "Name", value);
                }
            }
        }

        private int? _taskLimit;
        public int? TaskLimit
        {
            get => _taskLimit;
            set
            {
                _taskLimit = value;
                if (isPersisted && boardID.HasValue && columnIndex.HasValue)
                {
                    columnController.Update(boardID.Value, columnIndex.Value, "TaskLimit", value);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDAL"/> class.
        /// </summary>
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
        /// Persists the current instance by inserting it into the database if it hasn't been saved yet.
        /// </summary>
        public void Persist()
        {
            if (!isPersisted && boardID.HasValue && columnIndex.HasValue)
            {
                // Make sure your ColumnController's Insert method can access the 
                // columnIndex, either by passing it as a parameter or exposing an internal getter.
                columnController.Insert(boardID.Value, this);
                isPersisted = true;
            }
        }

        /// <summary>
        /// Adds a task to the column's task collection.
        /// </summary>
        /// <param name="task">The task data access object to add.</param>
        public void AddTask(TaskDAL task)
        {
            // Note: Adjust 'task.TaskID' if your TaskDAL property is simply named 'Id'
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