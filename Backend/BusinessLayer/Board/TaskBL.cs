using System;
using IntroSE.Kanban.Backend.BusinessLayer.CrossCutting;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer.Board
{
    /// <summary>
    /// Represents an individual task card on a Kanban board.
    /// </summary>
    internal class TaskBL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int maxTitleLength = 50;
        private const int maxDescriptionLength = 300;

        internal TaskDAL taskDTO;

        public long TaskID { get; }
        public DateTime CreationTime { get; }
        private string _title;
        public string Title
        {
            get => _title;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    string message = "Task title cannot be null or whitespace.";
                    log.Warn(message);
                    throw new KanbanValidationException(message);
                }
                if (value.Length > maxTitleLength)
                {
                    string message = $"Task title cannot exceed {maxTitleLength} characters. Attempted title length: {value.Length}.";
                    log.Warn(message);
                    throw new KanbanValidationException(message);
                }

                taskDTO.Title = value;
                _title = value;
            }
        }
        private DateTime _dueDate;
        public DateTime DueDate
        {
            get => _dueDate;
            private set
            {
                if (value < DateTime.Now)
                {
                    string message = $"Cannot set a due date in the past. Attempted date: {value}.";
                    log.Warn(message);
                    throw new KanbanValidationException(message);
                }

                taskDTO.DueDate = value;
                _dueDate = value;
            }
        }
        private string _description;
        public string Description
        {
            get => _description;
            private set
            {
                if (value?.Length > maxDescriptionLength)
                {
                    string message = $"Task description cannot exceed {maxDescriptionLength} characters. Attempted description length: {value.Length}.";
                    log.Warn(message);
                    throw new KanbanValidationException(message);
                }

                taskDTO.Description = value;
                _description = value;
            }
        }
        private string _assignee;
        public string Assignee
        {
            get => _assignee;
            set
            {
                taskDTO.Assignee = value;
                _assignee = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBL"/> class with the specified details.
        /// </summary>
        /// <param name="taskID">The unique identifier for the task.</param>
        /// <param name="title">The title of the task.</param>
        /// <param name="dueDate">The deadline/due date for the task.</param>
        /// <param name="description">A detailed description of the task.</param>
        public TaskBL(long taskID, string title, DateTime dueDate, string description)
        {
            this.CreationTime = DateTime.Now;
            this.taskDTO = new TaskDAL(taskID, this.CreationTime, title, dueDate, description, null);
            this.TaskID = taskID;
            this.Title = title;
            this.DueDate = dueDate;
            this.Description = description;
            this.Assignee = null;
            taskDTO.Persist();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBL"/> class based on an existing <see cref="TaskDAL"/> object.
        /// </summary>
        /// <param name="taskDTO">The data transfer object containing the task details.</param>
        public TaskBL(TaskDAL taskDTO)
        {
            this.taskDTO = taskDTO;
            this.TaskID = taskDTO.TaskID;
            this.CreationTime = taskDTO.CreationTime;

            this._title = taskDTO.Title;
            this._dueDate = taskDTO.DueDate;
            this._description = taskDTO.Description;
            this._assignee = taskDTO.Assignee;
        }

        /// <summary>
        /// Edits the details of the task, including its title, due date, and description.
        /// </summary>
        /// <param name="title">The new title for the task.</param>
        /// <param name="dueDate">The new due date for the task.</param>
        /// <param name="description">The new description for the task.</param>
        /// <returns>The updated <see cref="TaskBL"/> object.</returns>
        public TaskBL EditTask(string title, DateTime? dueDate, string description)
        {
            if (title == null && dueDate == null && description == null)
            {
                string message = $"Cannot edit task '{TaskID}' because no new values were provided.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }

            if (title != null)
            {
                Title = title;
            }
            if (dueDate.HasValue)
            {
                DueDate = dueDate.Value;
            }
            if (description != null)
            {
                Description = description;
            }

            return this;
        }
    }
}