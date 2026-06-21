using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// Controller responsible for executing database operations for column data.
    /// </summary>
    internal class ColumnController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string connectionString;
        private const string columnTableName = "Columns";
        private const string boardIdColumnName = "boardID";
        private const int boardIdColumnIndex = 0;
        private const string columnIndexColumnName = "columnIndex";
        private const int columnIndexColumnIndex = 1;
        private const string nameColumnName = "name";
        private const int nameColumnIndex = 2;
        private const string taskLimitColumnName = "taskLimit";
        private const int taskLimitColumnIndex = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnController"/> class and configures the database connection string.
        /// </summary>
        public ColumnController()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            this.connectionString = $"Data Source={path};Version=3;";
        }

        /// <summary>
        /// Inserts a new column record into the database.
        /// </summary>
        /// <param name="boardID">The ID of the board.</param>
        /// <param name="columnIndex">The positional index of the column.</param>
        /// <param name="column">The column data access object to insert.</param>
        public void Insert(long boardID, int columnIndex, ColumnDAL column)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"INSERT INTO {columnTableName} ({boardIdColumnName}, {columnIndexColumnName}, {nameColumnName}, {taskLimitColumnName}) VALUES (@BoardID, @ColumnIndex, @Name, @TaskLimit)";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@BoardID", boardID);
                        command.Parameters.AddWithValue("@ColumnIndex", columnIndex);
                        command.Parameters.AddWithValue("@Name", column.Name);
                        command.Parameters.AddWithValue("@TaskLimit", column.TaskLimit.HasValue ? column.TaskLimit.Value : DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                }
                log.Info($"Successfully inserted column '{column.Name}' at index {columnIndex} for board ID {boardID}.");
            }
            catch (SQLiteException)
            {
                string message = $"Failed to insert column '{column.Name}' at index {columnIndex} for board ID {boardID}.";
                log.Error(message);
                throw new DataException(message);
            }
        }

        /// <summary>
        /// Updates the task limit of a specific column.
        /// </summary>
        /// <param name="boardID">The ID of the board.</param>
        /// <param name="columnIndex">The index of the column to update.</param>
        /// <param name="limit">The new task limit.</param>
        public void UpdateTaskLimit(long boardID, int columnIndex, int? limit)
        {
            Update(boardID, columnIndex, taskLimitColumnName, limit.HasValue ? limit.Value : DBNull.Value);
        }

        /// <summary>
        /// Retrieves all columns associated with a specific board from the database.
        /// </summary>
        /// <param name="boardID">The ID of the board to query.</param>
        /// <returns>A list of <see cref="ColumnDAL"/> objects belonging to the board.</returns>
        public List<ColumnDAL> SelectBoardColumns(long boardID)
        {
            List<ColumnDAL> columns = new List<ColumnDAL>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"SELECT * FROM {columnTableName} WHERE {boardIdColumnName} = @BoardID";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@BoardID", boardID);
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                columns.Add(ConvertReaderToColumn(reader));
                            }
                        }
                    }
                }
                log.Debug($"Successfully retrieved {columns.Count} columns for board ID {boardID}.");
            }
            catch (SQLiteException)
            {
                string message = $"An error occurred while retrieving columns for board ID {boardID}.";
                log.Error(message);
                throw new DataException(message);
            }
            return columns;
        }

        /// <summary>
        /// Private helper method for updating a single attribute of a specific column.
        /// </summary>
        /// <param name="boardID">The ID of the board.</param>
        /// <param name="columnIndex">The index of the column.</param>
        /// <param name="attributeName">The database column name to update.</param>
        /// <param name="newValue">The new value to set.</param>
        private void Update(long boardID, int columnIndex, string attributeName, object newValue)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"UPDATE {columnTableName} SET {attributeName} = @NewValue WHERE {boardIdColumnName} = @BoardID AND {columnIndexColumnName} = @ColumnIndex";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@NewValue", newValue);
                        command.Parameters.AddWithValue("@BoardID", boardID);
                        command.Parameters.AddWithValue("@ColumnIndex", columnIndex);
                        command.ExecuteNonQuery();
                    }
                }
                log.Info($"Successfully updated {attributeName} for column at index {columnIndex} on board ID {boardID}.");
            }
            catch (SQLiteException)
            {
                string message = $"Failed to update {attributeName} for column at index {columnIndex} on board ID {boardID}.";
                log.Error(message);
                throw new DataException(message);
            }
        }

        /// <summary>
        /// Converts the current row of a SQLite data reader into a <see cref="ColumnDAL"/> object.
        /// </summary>
        /// <param name="reader">The active SQLite data reader.</param>
        /// <returns>A populated <see cref="ColumnDAL"/> instance.</returns>
        private ColumnDAL ConvertReaderToColumn(SQLiteDataReader reader)
        {
            long boardId = reader.GetInt64(boardIdColumnIndex);
            int colIndex = reader.GetInt32(columnIndexColumnIndex);
            string name = reader.GetString(nameColumnIndex);

            int? taskLimit = null;
            if (!reader.IsDBNull(taskLimitColumnIndex))
            {
                taskLimit = reader.GetInt32(taskLimitColumnIndex);
            }

            return new ColumnDAL(name, taskLimit, boardId, colIndex);
        }
    }
}