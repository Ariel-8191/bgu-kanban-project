using System;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Class representing a task in the service layer.
    /// </summary>
    public class TaskSL
    {
        public long taskID { get; }
        public DateTime creationTime { get; }
        public string title { get; }
        public DateTime dueDate { get; }
        public string description { get; }



        /// <summary>
        /// Initializes a new instance of the <see cref="TaskSL"/> class.
        /// </summary>
        /// <param name="taskBL"></param>
        internal TaskSL(TaskBL taskBL)
        {
            this.taskID = taskBL.taskID;
            this.creationTime = taskBL.creationTime;
            this.title = taskBL.title;
            this.dueDate = taskBL.dueDate;
            this.description = taskBL.description;
        }
    }
}