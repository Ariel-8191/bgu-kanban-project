using System;

namespace IntroSE.Kanban.Backend.BusinessLayer.Board
{
    /// <summary>
    /// Represents an individual task card on a Kanban board.
    /// </summary>
    internal class TaskBL
    {
        public long taskID;
        public DateTime creationTime;
        public string _title;
        public string Title
        {
            get { return _title; }
            private set
            {
                throw new NotImplementedException();
            }
        }
        public DateTime _dueDate;
        public DateTime DueDate
        {
            get { return _dueDate; }
            private set
            {
                throw new NotImplementedException();
            }
        }
        public string _description;
        public string Description
        {
            get { return _description; }
            private set
            {
                throw new NotImplementedException();
            }
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
            throw new NotImplementedException();
        }
    }
}
