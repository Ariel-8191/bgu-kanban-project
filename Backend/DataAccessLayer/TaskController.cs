using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// Controller responsible for executing database operations for task data.
    /// </summary>
    internal class TaskController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string connectionString;

        private const string taskTableName = "Tasks";
        private const string boardIdColumnName = "boardID";
        private const int boardIdColumnIndex = 0;
        private const string taskIdColumnName = "taskID";
        private const int taskIdColumnIndex = 1;
        private const string columnIndexColumnName = "columnIndex";
        private const int columnIndexColumnIndex = 2;
        private const string creationDateColumnName = "creationDate";
        private const int creationDateColumnIndex = 3;
        private const string titleColumnName = "title";
        private const int titleColumnIndex = 4;
        private const string dueDateColumnName = "dueDate";
        private const int dueDateColumnIndex = 5;
        private const string descriptionColumnName = "description";
        private const int descriptionColumnIndex = 6;
        private const string assigneeColumnName = "assignee";
        private const int assigneeColumnIndex = 7;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskController"/> class and configures the database connection string.
        /// </summary>
        public TaskController()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            this.connectionString = $"Data Source={path};Version=3;";
        }

        /// <summary>
        /// Inserts a new task record into the database.
        /// </summary>
        public void Insert(long boardID, int columnIndex, TaskDAL task)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"INSERT INTO {taskTableName} ({boardIdColumnName}, {taskIdColumnName}, {columnIndexColumnName}, {creationDateColumnName}, {dueDateColumnName}, {assigneeColumnName}, {titleColumnName}, {descriptionColumnName}) VALUES (@BoardID, @TaskID, @ColumnIndex, @CreationDate, @DueDate, @Assignee, @Title, @Description) ON CONFLICT(BoardID, TaskID) DO UPDATE SET ColumnIndex = excluded.ColumnIndex;";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@BoardID", boardID);
                        command.Parameters.AddWithValue("@TaskID", task.TaskID);
                        command.Parameters.AddWithValue("@ColumnIndex", columnIndex);
                        command.Parameters.AddWithValue("@CreationDate", task.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@DueDate", task.DueDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@Assignee", string.IsNullOrEmpty(task.Assignee) ? DBNull.Value : (object)task.Assignee);
                        command.Parameters.AddWithValue("@Title", task.Title);
                        command.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(task.Description) ? DBNull.Value : (object)task.Description);
                        command.ExecuteNonQuery();
                    }
                }
                log.Info($"Successfully inserted task ID {task.TaskID} into board ID {boardID}.");
            }
            catch (SQLiteException)
            {
                string message = $"Failed to insert task ID {task.TaskID} into board ID {boardID}.";
                log.Error(message);
                throw new DataException(message);
            }
        }

        /// <summary>
        /// Updates the title of a specific task.
        /// </summary>
        public void UpdateTitle(long boardID, long taskID, string title)
        {
            Update(boardID, taskID, titleColumnName, title);
        }

        /// <summary>
        /// Updates the due date of a specific task.
        /// </summary>
        public void UpdateDueDate(long boardID, long taskID, DateTime dueDate)
        {
            Update(boardID, taskID, dueDateColumnName, dueDate.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// Updates the description of a specific task.
        /// </summary>
        public void UpdateDescription(long boardID, long taskID, string description)
        {
            Update(boardID, taskID, descriptionColumnName, string.IsNullOrEmpty(description) ? DBNull.Value : (object)description);
        }

        /// <summary>
        /// Updates the assignee of a specific task.
        /// </summary>
        public void UpdateAssignee(long boardID, long taskID, string assignee)
        {
            Update(boardID, taskID, assigneeColumnName, string.IsNullOrEmpty(assignee) ? DBNull.Value : (object)assignee);
        }

        /// <summary>
        /// Retrieves all tasks associated with a specific column from the database.
        /// </summary>
        public List<TaskDAL> SelectColumnTasks(long boardID, int columnIndex)
        {
            List<TaskDAL> tasks = new List<TaskDAL>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"SELECT * FROM {taskTableName} WHERE {boardIdColumnName} = @BoardID AND {columnIndexColumnName} = @ColumnIndex";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@BoardID", boardID);
                        command.Parameters.AddWithValue("@ColumnIndex", columnIndex);
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tasks.Add(ConvertReaderToTask(reader));
                            }
                        }
                    }
                }
                log.Debug($"Successfully retrieved {tasks.Count} tasks for column {columnIndex} on board ID {boardID}.");
            }
            catch (SQLiteException)
            {
                string message = $"An error occurred while retrieving tasks for column {columnIndex} on board ID {boardID}.";
                log.Error(message);
                throw new DataException(message);
            }
            return tasks;
        }

        /// <summary>
        /// Private helper method for updating a single attribute of a specific task.
        /// </summary>
        private void Update(long boardID, long taskID, string attributeName, object newValue)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"UPDATE {taskTableName} SET {attributeName} = @NewValue WHERE {boardIdColumnName} = @BoardID AND {taskIdColumnName} = @TaskID";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@NewValue", newValue);
                        command.Parameters.AddWithValue("@BoardID", boardID);
                        command.Parameters.AddWithValue("@TaskID", taskID);
                        command.ExecuteNonQuery();
                    }
                }
                log.Info($"Successfully updated {attributeName} for task ID {taskID} on board ID {boardID}.");
            }
            catch (SQLiteException)
            {
                string message = $"Failed to update {attributeName} for task ID {taskID} on board ID {boardID}.";
                log.Error(message);
                throw new DataException(message);
            }
        }

        /// <summary>
        /// Converts the current row of a SQLite data reader into a <see cref="TaskDAL"/> object.
        /// </summary>
        private TaskDAL ConvertReaderToTask(SQLiteDataReader reader)
        {
            long boardId = reader.GetInt64(boardIdColumnIndex);
            long taskId = reader.GetInt64(taskIdColumnIndex);
            int colIndex = reader.GetInt32(columnIndexColumnIndex);
            DateTime creationDate = DateTime.Parse(reader.GetString(creationDateColumnIndex));
            DateTime dueDate = DateTime.Parse(reader.GetString(dueDateColumnIndex));

            string assignee = null;
            if (!reader.IsDBNull(assigneeColumnIndex))
            {
                assignee = reader.GetString(assigneeColumnIndex);
            }

            string title = reader.GetString(titleColumnIndex);

            string description = null;
            if (!reader.IsDBNull(descriptionColumnIndex))
            {
                description = reader.GetString(descriptionColumnIndex);
            }

            return new TaskDAL(taskId, creationDate, title, description, dueDate, assignee, boardId, colIndex);
        }
    }
}