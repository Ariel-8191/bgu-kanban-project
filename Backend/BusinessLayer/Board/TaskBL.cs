using System;
using IntroSE.Kanban.Backend.BusinessLayer.CrossCutting;

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

                _title = value;
            }
        }
        public DateTime DueDate { get; private set; }
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

                _description = value;
            }
        }
        public string Assignee { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBL"/> class with the specified details.
        /// </summary>
        /// <param name="taskID">The unique identifier for the task.</param>
        /// <param name="title">The title of the task.</param>
        /// <param name="dueDate">The deadline/due date for the task.</param>
        /// <param name="description">A detailed description of the task.</param>
        public TaskBL(long taskID, string title, DateTime dueDate, string description)
        {
            this.TaskID = taskID;
            this.CreationTime = DateTime.Now;
            this.Title = title;
            this.DueDate = dueDate;
            this.Description = description;
            this.Assignee = null;
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
