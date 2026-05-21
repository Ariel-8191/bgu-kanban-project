using System;
using IntroSE.Kanban.Backend.BusinessLayer.Board;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Represents a task in the service layer.
    /// </summary>
    public class TaskSL
    {
        public long TaskID { get; }
        public DateTime CreationTime { get; }
        public string Title { get; }
        public DateTime DueDate { get; }
        public string Description { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskSL"/> class.
        /// </summary>
        /// <param name="taskBL">The task instance from the businnes layer that the new instance represents</param>
        internal TaskSL(TaskBL taskBL)
        {
            this.TaskID = taskBL.TaskID;
            this.CreationTime = taskBL.CreationTime;
            this.Title = taskBL.Title;
            this.DueDate = taskBL.DueDate;
            this.Description = taskBL.Description;
        }
    }
}