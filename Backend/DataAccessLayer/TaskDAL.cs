using System;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// Represents a task within the data access layer of the Kanban application.
    /// </summary>
    internal class TaskDAL
    {
        private TaskController taskController;
        private bool isPersisted;
        private long? boardID;
        private int? columnIndex;

        public long TaskID { get; }
        public DateTime CreationTime { get; }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (isPersisted)
                {
                    taskController.UpdateTitle(boardID.Value, TaskID, value);
                }
                _title = value;
            }
        }

        private DateTime _dueDate;
        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                if (isPersisted)
                {
                    taskController.UpdateDueDate(boardID.Value, TaskID, value);
                }
                _dueDate = value;
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (isPersisted)
                {
                    taskController.UpdateDescription(boardID.Value, TaskID, value);
                }
                _description = value;
            }
        }

        private string _assignee;
        public string Assignee
        {
            get => _assignee;
            set
            {
                if (isPersisted)
                {
                    taskController.UpdateAssignee(boardID.Value, TaskID, value);
                }
                _assignee = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDAL"/> class.
        /// </summary>
        public TaskDAL(long taskID, DateTime creationTime, string title, string description, DateTime dueDate, string assignee)
        {
            this.taskController = new TaskController();
            this.isPersisted = false;
            this.TaskID = taskID;
            this.CreationTime = creationTime;
            this.Title = title;
            this.Description = description;
            this.DueDate = dueDate;
            this.Assignee = assignee;
        }

        /// <summary>
        /// Loads a persisted instance of the <see cref="TaskDAL"/> class from the database.
        /// </summary>
        internal TaskDAL(long taskID, DateTime creationTime, string title, string description, DateTime dueDate, string assignee, long boardID, int columnIndex)
            : this(taskID, creationTime, title, description, dueDate, assignee)
        {
            this.boardID = boardID;
            this.columnIndex = columnIndex;
            this.isPersisted = true;
        }

        /// <summary>
        /// A 'fake' persist that ensures the BL isn't aware of the database implementation details.
        /// </summary>
        public void Persist() { }

        /// <summary>
        /// Persists the current instance by inserting it into the database and setting its boardID and columnIndex.
        /// </summary>
        public void Persist(long boardID, int columnIndex)
        {
            if (!isPersisted)
            {
                this.boardID = boardID;
                this.columnIndex = columnIndex;
                taskController.Insert(boardID, columnIndex, this);
                isPersisted = true;
            }
        }
    }
}