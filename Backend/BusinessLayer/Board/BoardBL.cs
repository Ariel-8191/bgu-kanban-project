using IntroSE.Kanban.Backend.BusinessLayer.CrossCutting;
using IntroSE.Kanban.Backend.BusinessLayer.CrossCutting;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.BusinessLayer.Board
{
    /// <summary>
    /// Represents a Kanban board in the business layer, managing its columns, tasks, and properties.
    /// </summary>
    internal class BoardBL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal const int BacklogColumnIndex = 0;
        internal const int InProgressColumnIndex = 1;
        internal const int DoneColumnIndex = 2;
        internal const string BacklogColumnName = "backlog";
        internal const string InProgressColumnName = "in progress";
        internal const string DoneColumnName = "done";

        private BoardDAL boardDTO;

        public long BoardID { get; }
        public string BoardName { get; }
        private string _owner;
        internal string Owner
        {
            get => _owner;
            set
            {
                boardDTO.Owner = value;
                _owner = value;
            }
        }
        internal HashSet<string> members;
        private List<ColumnBL> columns;
        private long nextTaskID;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardBL"/> class with the specified board name.
        /// </summary>
        /// <param name="boardID">The ID to be assigned to the new board.</param>
        /// <param name="boardName">The name to be assigned to the new board.</param>
        /// <param name="creator">The email address of the user who created the board.</param>
        public BoardBL(long boardID, string boardName, string creator)
        {
            this.boardDTO = new BoardDAL(boardID, boardName, creator, 0);
            this.BoardID = boardID;
            this.BoardName = boardName;
            this._owner = creator;
            this.members = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { creator };
            this.nextTaskID = 0;


            boardDTO.Persist();

            this.columns = new List<ColumnBL>();
            CreateColumn(BacklogColumnName);
            CreateColumn(InProgressColumnName);
            CreateColumn(DoneColumnName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardBL"/> class using an existing <see cref="BoardDAL"/> object.
        /// </summary>
        /// <param name="boardDTO">The data access layer object representing the existing board.</param>
        public BoardBL(BoardDAL boardDTO)
        {
            this.boardDTO = boardDTO;
            this.BoardID = boardDTO.BoardID;
            this.BoardName = boardDTO.BoardName;
            this._owner = boardDTO.Owner;
            this.members = boardDTO.Members;
            this.nextTaskID = boardDTO.NextTaskID;
            this.columns = boardDTO.Columns.ConvertAll(columnDTO => new ColumnBL(columnDTO));
        }

        /// <summary>
        /// Creates a new column with the specified name and adds it to the board. The new column is also persisted in the data access layer.
        /// </summary>
        /// <param name="name">The name of the column to create.</param>
        private void CreateColumn(string name)
        {
            ColumnBL column = new ColumnBL(name);
            int columnIndex = columns.Count;
            boardDTO.CreateColumn(columnIndex, column.columnDTO);
            columns.Add(column);
        }

        /// <summary>
        /// Adds a user to the board
        /// </summary>
        /// <param name="email">The email of the user to add</param>
        public void AddMember(string email)
        {
            boardDTO.AddMember(email);
            members.Add(email);
        }

        /// <summary>
        /// Removes a user to the board
        /// </summary>
        /// <param name="email">The email of the user to add</param>
        public void RemoveMember(string email)
        {
            if (!members.Contains(email))
            {
                string message = $"Can't remove a user from '{BoardName}' because user '{email}' is not in it.";
                log.Warn(message);
                throw new KanbanNotFoundException(message);
            }
            if (Owner.Equals(email, StringComparison.OrdinalIgnoreCase))
            {
                string message = $"Can't remove a user from '{BoardName}' because user '{email}' is the owner of the board.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }

            UnassignMemberTasks(email);

            members.Remove(email);
        }

        /// <summary>
        /// Unassigns member from all of his assigned tasks that are not done.
        /// </summary>
        /// <param name="email">The email of the user to unassign from all the tasks</param>
        private void UnassignMemberTasks(string email)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                if (i == DoneColumnIndex)
                {
                    continue; //Skip the tasks that are done
                }

                foreach (TaskBL task in columns[i].GetTasks())
                {
                    if (task.Assignee.Equals(email, StringComparison.OrdinalIgnoreCase))
                    {
                        task.Assignee = null;
                    }
                }
            }

            boardDTO.RemoveMember(email);
            members.Remove(email);
        }

        /// <summary>
        /// Transfers the ownership of the board to a new user.
        /// </summary>
        /// <param name="currentOwnerEmail">The email of the current owner.</param>
        /// <param name="newOwnerEmail">The email of the new owner.</param>
        public void TransferOwnership(string currentOwnerEmail, string newOwnerEmail)
        {
            if (!this.Owner.Equals(currentOwnerEmail, StringComparison.OrdinalIgnoreCase))
            {
                string message = $"Cannot transfer ownership of board '{BoardName}' because user '{currentOwnerEmail}' is not the owner.";
                log.Warn(message);
                throw new KanbanAuthenticationException(message);
            }

            if (!this.members.Contains(newOwnerEmail))
            {
                string message = $"Cannot transfer ownership of board '{BoardName}' to user '{newOwnerEmail}' because they have not joined the board.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }

            if (currentOwnerEmail.Equals(newOwnerEmail, StringComparison.OrdinalIgnoreCase))
            {
                string message = $"Cannot transfer ownership of board '{BoardName}' to the same user.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }

            this.Owner = newOwnerEmail;
        }

        /// <summary>
        /// Sets or removes the maximum limit on the number of tasks allowed in a specific column.
        /// </summary>
        /// <param name="columnIndex">The zero-based index of the column to modify.</param>
        /// <param name="limit">The maximum number of tasks allowed, or null to remove the limit.</param>
        public void LimitTasksInColumn(int columnIndex, int? limit)
        {
            if (columnIndex < 0 || columnIndex >= columns.Count)
            {
                string message = $"Cannot set the task limit of a column in the board '{BoardName}' because the column index is out of bounds.";
                log.Warn(message);
                throw new KanbanNotFoundException(message);
            }

            columns[columnIndex].TaskLimit = limit;
        }

        /// <summary>
        /// Retrieves a list of all tasks currently located in a specific column.
        /// </summary>
        /// <param name="columnIndex">The zero-based index of the column.</param>
        /// <returns>A list of <see cref="TaskBL"/> objects present in the specified column.</returns>
        public List<TaskBL> GetColumnTasks(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= columns.Count)
            {
                string message = $"Cannot get the tasks of a column in the board '{BoardName}' because the column index is out of bounds.";
                log.Warn(message);
                throw new KanbanNotFoundException(message);
            }

            return columns[columnIndex].GetTasks();
        }

        /// <summary>
        /// Retrieves the name of a specific column on the board.
        /// </summary>
        /// <param name="columnIndex">The zero-based index of the column.</param>
        /// <returns>The name of the column as a string.</returns>
        public string GetColumnName(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= columns.Count)
            {
                string message = $"Cannot get the name of a column in the board '{BoardName}' because the column index is out of bounds.";
                log.Warn(message);
                throw new KanbanNotFoundException(message);
            }
            return columns[columnIndex].Name;
        }

        /// <summary>
        /// Gets the maximum number of tasks allowed in a specific column.
        /// </summary>
        /// <param name="columnIndex">The zero-based index of the column to query.</param>
        /// <returns>An integer representing the task limit, or null if there is no limit set.</returns>
        public int? GetColumnLimit(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= columns.Count)
            {
                string message = $"Cannot get the task limit of a column in the board '{BoardName}' because the column index is out of bounds.";
                log.Warn(message);
                throw new KanbanNotFoundException(message);
            }

            return columns[columnIndex].TaskLimit;
        }

        /// <summary>
        /// Creates and adds a new task to the board. 
        /// </summary>
        /// <param name="email">The email of the user trying to add a task</param>
        /// <param name="title">The title of the new task.</param>
        /// <param name="dueDate">The deadline/due date for the task.</param>
        /// <param name="description">A detailed description of the task.</param>
        /// <returns>The newly created <see cref="TaskBL"/> object.</returns>
        public TaskBL AddTask(string email, string title, DateTime dueDate, string description)
        {
            if (!members.Contains(email))
            {
                string message = $"Cannot add task because the given user isn't a member of the board.";
                log.Warn(message);
                throw new KanbanAuthenticationException(message);
            }
            TaskBL newTask = new TaskBL(nextTaskID, title, dueDate, description);
            nextTaskID++;
            boardDTO.NextTaskID = nextTaskID;

            columns[BacklogColumnIndex].AddTask(newTask);

            return newTask;
        }

        /// <summary>
        /// Edits the details of an existing task on the board.
        /// </summary>
        /// <param name="email">The email address of the user editing the task.</param>
        /// <param name="taskID">The unique identifier of the task to edit.</param>
        /// <param name="title">The new title for the task.</param>
        /// <param name="dueDate">The new due date for the task.</param>
        /// <param name="description">The new description for the task.</param>
        /// <returns>The updated <see cref="TaskBL"/> object.</returns>
        public TaskBL EditTask(string email, int columnIndex, long taskID, string title, DateTime? dueDate, string description)
        {
            if (columnIndex < 0 || columnIndex >= columns.Count)
            {
                string message = $"Cannot edit task '{taskID}' in the board '{BoardName}' in column {columnIndex} because the column index is out of bounds.";
                log.Warn(message);
                throw new KanbanNotFoundException(message);
            }
            if (!columns[columnIndex].ContainsTask(taskID))
            {
                string message = $"Cannot edit task '{taskID}' in the board '{BoardName}' in column {columnIndex} because it does not exist.";
                log.Warn(message);
                throw new KanbanNotFoundException(message);
            }

            TaskBL taskToEdit = columns[columnIndex].GetTask(taskID);
            if (columnIndex == DoneColumnIndex)
            {
                string message = $"Cannot edit task '{taskID}' in the board '{BoardName}' because it is in the \"Done\" column";
                log.Warn(message);
                throw new KanbanInvalidStateException(message);
            }
            if (!email.Equals(taskToEdit.Assignee, StringComparison.OrdinalIgnoreCase) && !email.Equals(Owner, StringComparison.OrdinalIgnoreCase))
            {
                string message = $"Cannot edit task '{taskID}' in the board '{BoardName}' because the user '{email}' is neither the assignee nor the owner.";
                log.Warn(message);
                throw new KanbanAuthenticationException(message);
            }

            taskToEdit.EditTask(title, dueDate, description);
            return taskToEdit;
        }

        /// <summary>
        /// Advances a specific task from its current column to the next column on the board.
        /// </summary>
        /// <param name="email">The email address of the user advancing the task.</param>
        /// <param name="columnIndex">The index of the column where the task is currently located.</param>
        /// <param name="taskID">The unique identifier of the task to advance.</param>
        /// <returns>The updated <see cref="TaskBL"/> object reflecting its new state/column.</returns>
        public TaskBL AdvanceTask(string email, int columnIndex, long taskID)
        {
            if (columnIndex < 0 || columnIndex >= columns.Count)
            {
                string message = $"Cannot advance task '{taskID}' in the board '{BoardName}' from column {columnIndex} because the column index is out of bounds.";
                log.Warn(message);
                throw new KanbanNotFoundException(message);
            }
            if (!columns[columnIndex].ContainsTask(taskID))
            {
                string message = $"Cannot advance task '{taskID}' in the board '{BoardName}' in column {columnIndex} because it does not exist.";
                log.Warn(message);
                throw new KanbanNotFoundException(message);
            }


            TaskBL taskToAdvance = columns[columnIndex].GetTask(taskID);
            if (columnIndex + 1 >= columns.Count)
            {
                string message = $"Cannot advance task '{taskID}' in the board '{BoardName}' from column {columnIndex} because there is nowhere to advance.";
                log.Warn(message);
                throw new KanbanInvalidStateException(message);
            }
            if (!email.Equals(taskToAdvance.Assignee, StringComparison.OrdinalIgnoreCase))
            {
                string message = $"Cannot advance task '{taskID}' in the board '{BoardName}' because the user '{email}' is not the assignee.";
                log.Warn(message);
                throw new KanbanAuthenticationException(message);
            }

            // Adding the task to the next column before removing it from the current one ensures that the task won't disapper if the next column is full
            columns[columnIndex + 1].AddTask(taskToAdvance);
            columns[columnIndex].RemoveTask(taskToAdvance);
            return taskToAdvance;
        }

        /// <summary>
        /// Assigns a task to a new assignee.
        /// </summary>
        /// <param name="columnIndex">The index of the column where the task is currently located.</param>
        /// <param name="taskID">The unique identifier of the task to assign.</param>
        /// <param name="assigner">The user attempting to assign the task.</param>
        /// <param name="newAssignee">The user to whom the task is being assigned.</param>
        /// <returns>The updated <see cref="TaskBL"/> object reflecting its new assignee.</returns>
        public TaskBL AssignTask(int columnIndex, long taskID, string assigner, string newAssignee)
        {
            if (columnIndex < 0 || columnIndex >= columns.Count)
            {
                string message = $"Cannot assign task '{taskID}' in the board '{BoardName}' from column {columnIndex} because the column index is out of bounds.";
                log.Warn(message);
                throw new KanbanNotFoundException(message);
            }
            if (!columns[columnIndex].ContainsTask(taskID))
            {
                string message = $"Cannot assign task '{taskID}' in the board '{BoardName}' in column {columnIndex} because it does not exist.";
                log.Warn(message);
                throw new KanbanNotFoundException(message);
            }
            if (!members.Contains(assigner))
            {
                string message = $"Cannot assign task '{taskID}' in the board '{BoardName}' in column {columnIndex} because the user '{assigner}' is not a member of the board.";
                log.Warn(message);
                throw new KanbanAuthenticationException(message);
            }
            if (!members.Contains(newAssignee))
            {
                string message = $"Cannot assign task '{taskID}' in the board '{BoardName}' in column {columnIndex} to user '{newAssignee}' because they are not a member of the board.";
                log.Warn(message);
                throw new KanbanAuthenticationException(message);
            }
            if (assigner.Equals(newAssignee, StringComparison.OrdinalIgnoreCase))
            {
                string message = $"Cannot transfer ownership of board '{BoardName}' to the same user.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }

            TaskBL taskToAssign = columns[columnIndex].GetTask(taskID);
            string currentAssignee = taskToAssign.Assignee;

            // If the task is unassigned OR the user trying to assign is the current assignee OR the user trying to assign is the owner of the board, allow the assignment
            if (currentAssignee == null || assigner.Equals(currentAssignee, StringComparison.OrdinalIgnoreCase) || assigner.Equals(Owner, StringComparison.OrdinalIgnoreCase))
            {
                taskToAssign.Assignee = newAssignee;
                return taskToAssign;
            }
            else
            {
                string message = $"Cannot assign task '{taskID}' in the board '{BoardName}' in column {columnIndex} because the user '{assigner}' is not the current assignee or the owner.";
                log.Warn(message);
                throw new KanbanInvalidStateException(message);
            }
        }
    }
}
