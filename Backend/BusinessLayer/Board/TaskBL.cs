using System;

namespace IntroSE.Kanban.Backend.BusinessLayer.Board
{
    /// <summary>
    /// Represents an individual task card on a Kanban board.
    /// </summary>
    internal class TaskBL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public long TaskID { get; }
        public DateTime CreationTime { get; }
        public string _title;
        public string Title
        {
            get { return _title; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    log.Warn("Invalid title: Value is null or whitespace.");
                    throw new ArgumentException("Title cannot be null or whitespace.", nameof(value));
                }
                if (value.Length > 50)
                {
                    log.WarnFormat("Invalid title: Length of '{0}' exceeds maximum of 50 characters.", nameof(value));
                    throw new ArgumentException("Title cannot exceed 50 characters.", nameof(value));
                }

                _title = value;
            }
        }
        public DateTime DueDate { get; private set; }
        public string _description;
        public string Description
        {
            get { return _description; }
            private set
            {
                if (value?.Length > 300)
                {
                    log.WarnFormat("Invalid description: Length of '{0}' exceeds maximum of 300 characters.", nameof(value));
                    throw new ArgumentException("Description cannot exceed 300 characters.", nameof(value));
                }

                _description = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBL"/> class with the specified details.
        /// </summary>
        /// param name="taskID">The unique identifier for the task.</param>
        /// param name="title">The title of the task.</param>
        /// param name="dueDate">The deadline/due date for the task.</param>
        /// param name="description">A detailed description of the task.</param>
        public TaskBL(long taskID, string title, DateTime dueDate, string description)
        {
            this.TaskID = taskID;
            this.CreationTime = DateTime.Now;
            this.Title = title;
            this.DueDate = dueDate;
            this.Description = description;
        }

        /// <summary>
        /// Edits the details of the task, including its title, due date, and description.
        /// </summary>
        /// <param name="title">The new title for the task.</param>
        /// <param name="dueDate">The new due date for the task.</param>
        /// <param name="description">The new description for the task.</param>
        /// <returns>The updated <see cref="TaskBL"/> object.</returns>
        public TaskBL EditTask(string title, DateTime dueDate, string description)
        {
            if (title != null)
            {
                Title = title;
            }
            if (dueDate != default)
            {
                DueDate = dueDate;
            }
            if (description != null)
            {
                Description = description;
            }

            return this;
        }
    }
}
