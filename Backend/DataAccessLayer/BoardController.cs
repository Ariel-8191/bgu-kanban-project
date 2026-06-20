using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// Controller responsible for executing database operations for board data.
    /// </summary>
    internal class BoardController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string connectionString;
        private const string boardTableName = "Boards";
        private const string idColumnName = "BoardID";
        private const int idColumnIndex = 0;
        private const string nameColumnName = "BoardName";
        private const int nameColumnIndex = 1;
        private const string ownerColumnName = "Owner";
        private const int ownerColumnIndex = 2;
        private const string nextTaskIdColumnName = "next_task_id";
        private const int nextTaskIdColumnIndex = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardController"/> class and configures the database connection string.
        /// </summary>
        public BoardController()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            this.connectionString = $"Data Source={path};Version=3;";
        }

        /// <summary>
        /// Inserts a new board record into the database.
        /// </summary>
        /// <param name="board">The board data access object containing the details to insert.</param>
        public void Insert(BoardDAL board)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"INSERT INTO {boardTableName} ({idColumnName}, {nameColumnName}, {ownerColumnName}, {nextTaskIdColumnName}) VALUES (@BoardID, @BoardName, @Owner, @NextTaskID)";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@BoardID", board.BoardID);
                        command.Parameters.AddWithValue("@BoardName", board.BoardName);
                        command.Parameters.AddWithValue("@Owner", board.Owner);
                        command.Parameters.AddWithValue("@NextTaskID", board.NextTaskID);
                        command.ExecuteNonQuery();
                    }
                }
                log.Info($"Successfully inserted board '{board.BoardName}' (ID: {board.BoardID}) into the database.");
            }
            catch (SQLiteException)
            {
                string message = $"Failed to insert board '{board.BoardName}' (ID: {board.BoardID}) into the database.";
                log.Error(message);
                throw new DataException(message);
            }
        }

        /// <summary>
        /// Deletes a specific board record from the database.
        /// </summary>
        /// <param name="board">The board data access object to delete.</param>
        public void Delete(BoardDAL board)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"DELETE FROM {boardTableName} WHERE {idColumnName} = @BoardID";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@BoardID", board.BoardID);
                        command.ExecuteNonQuery();
                    }
                }
                log.Info($"Successfully deleted board ID {board.BoardID} from the database.");
            }
            catch (SQLiteException)
            {
                string message = $"Failed to delete board ID {board.BoardID} from the database.";
                log.Error(message);
                throw new DataException(message);
            }
        }

        /// <summary>
        /// Updates the owner of a specific board.
        /// </summary>
        /// <param name="boardID">The ID of the board to update.</param>
        /// <param name="newOwnerEmail">The email of the new owner.</param>
        public void UpdateOwner(long boardID, string newOwnerEmail)
        {
            Update(boardID, ownerColumnName, newOwnerEmail);
        }

        /// <summary>
        /// Updates the next task ID of a specific board.
        /// </summary>
        /// <param name="boardID">The ID of the board to update.</param>
        /// <param name="nextTaskID">The new NextTaskID value.</param>
        public void UpdateNextTaskID(long boardID, long nextTaskID)
        {
            Update(boardID, nextTaskIdColumnName, nextTaskID);
        }

        /// <summary>
        /// Retrieves all boards from the database.
        /// </summary>
        /// <returns>A list of <see cref="BoardDAL"/> objects representing all boards.</returns>
        public List<BoardDAL> SelectAll()
        {
            List<BoardDAL> boards = new List<BoardDAL>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"SELECT * FROM {boardTableName}";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                boards.Add(ConvertReaderToBoard(reader));
                            }
                        }
                    }
                }
                log.Info($"Successfully retrieved {boards.Count} boards from the database.");
            }
            catch (SQLiteException)
            {
                string message = "An error occurred while retrieving all boards from the database.";
                log.Error(message);
                throw new DataException(message);
            }
            return boards;
        }

        /// <summary>
        /// Deletes all board records from the database.
        /// </summary>
        public void DeleteAll()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"DELETE FROM {boardTableName}";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        log.Info($"Successfully deleted all boards from the database. Rows affected: {rowsAffected}");
                    }
                }
            }
            catch (SQLiteException)
            {
                string message = "An error occurred while attempting to delete all boards from the database.";
                log.Error(message);
                throw new DataException(message);
            }
        }

        /// <summary>
        /// Calculates and retrieves the next available ID for a new board.
        /// </summary>
        /// <returns>The next available board ID.</returns>
        public long GetNextAvailableBoardID()
        {
            long nextId = 0;
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"SELECT MAX({idColumnName}) FROM {boardTableName}";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        object result = command.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            nextId = Convert.ToInt64(result) + 1;
                        }
                    }
                }
                log.Info($"Next available board ID calculated as {nextId}.");
            }
            catch (SQLiteException)
            {
                string message = "An error occurred while calculating the next available board ID.";
                log.Error(message);
                throw new DataException(message);
            }
            return nextId;
        }

        /// <summary>
        /// Private helper method for updating a single attribute of a specific board.
        /// </summary>
        /// <param name="boardID">The ID of the board to update.</param>
        /// <param name="attributeName">The column name to update.</param>
        /// <param name="newValue">The new value to set.</param>
        private void Update(long boardID, string attributeName, object newValue)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"UPDATE {boardTableName} SET {attributeName} = @NewValue WHERE {idColumnName} = @BoardID";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@NewValue", newValue);
                        command.Parameters.AddWithValue("@BoardID", boardID);
                        command.ExecuteNonQuery();
                    }
                }
                log.Info($"Successfully updated {attributeName} for board ID {boardID}.");
            }
            catch (SQLiteException)
            {
                string message = $"Failed to update {attributeName} for board ID {boardID}.";
                log.Error(message);
                throw new DataException(message);
            }
        }

        /// <summary>
        /// Converts the current row of a SQLite data reader into a <see cref="BoardDAL"/> object.
        /// </summary>
        /// <param name="reader">The active SQLite data reader.</param>
        /// <returns>A populated <see cref="BoardDAL"/> instance.</returns>
        private BoardDAL ConvertReaderToBoard(SQLiteDataReader reader)
        {
            long id = reader.GetInt64(idColumnIndex);
            string name = reader.GetString(nameColumnIndex);
            string owner = reader.GetString(ownerColumnIndex);
            long nextTaskID = reader.GetInt64(nextTaskIdColumnIndex);

            return new BoardDAL(id, name, owner, nextTaskID);
        }
    }
}
