using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO.Compression;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// Controller responsible for executing database operations for the BoardsUsers many-to-many relationship.
    /// </summary>
    internal class BoardsUsersController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string connectionString;

        private const string tableName = "BoardsUsers";
        private const string boardIdColumnName = "boardID";
        private const string emailColumnName = "email";

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardsUsersController"/> class and configures the database connection string.
        /// </summary>
        public BoardsUsersController()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            this.connectionString = $"Data Source={path};Version=3;";
        }

        /// <summary>
        /// Assigns a user to a board by inserting a record into the BoardsUsers table.
        /// </summary>
        /// <param name="boardID">The ID of the board.</param>
        /// <param name="email">The email of the user joining the board.</param>
        public void Insert(long boardID, string email)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"INSERT INTO {tableName} ({boardIdColumnName}, {emailColumnName}) VALUES (@BoardID, @Email)";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@BoardID", boardID);
                        command.Parameters.AddWithValue("@Email", email);
                        command.ExecuteNonQuery();
                    }
                }
                log.Info($"Successfully added user '{email}' to board ID {boardID}.");
            }
            catch (SQLiteException ex)
            {
                string message = $"Failed to add user '{email}' to board ID {boardID}.";
                log.Error(message, ex);
                throw new DataException(message);
            }
        }

        /// <summary>
        /// Removes a user from a board by deleting the corresponding record from the BoardsUsers table.
        /// </summary>
        /// <param name="boardID">The ID of the board.</param>
        /// <param name="email">The email of the user leaving the board.</param>
        public void Delete(long boardID, string email)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"DELETE FROM {tableName} WHERE {boardIdColumnName} = @BoardID AND {emailColumnName} = @Email";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@BoardID", boardID);
                        command.Parameters.AddWithValue("@Email", email);
                        command.ExecuteNonQuery();

                        log.Info($"Successfully removed user '{email}' from board ID {boardID}.");                     
                    }
                }
            }
            catch (SQLiteException)
            {
                string message = $"Failed to remove user '{email}' from board ID {boardID}.";
                log.Error(message);
                throw new DataException(message);
            }
        }

        /// <summary>
        /// Retrieves all members in a specific board.
        /// </summary>
        /// <param name="boardID">The ID of the board to query.</param>
        /// <returns>A HashSet containing the email addresses of all board members.</returns>
        public HashSet<string> GetBoardMembers(long boardID)
        {
            HashSet<string> members = new HashSet<string>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"SELECT {emailColumnName} FROM {tableName} WHERE {boardIdColumnName} = @BoardID";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@BoardID", boardID);
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //We only ask for one column so the index is 0 (this isn't a magic number)
                                members.Add(reader.GetString(0)); 
                            }
                        }
                    }
                }
                log.Info($"Successfully retrieved {members.Count} members for board ID {boardID}.");
            }
            catch (SQLiteException)
            {
                string message = $"An error occurred while retrieving members for board ID {boardID}.";
                log.Error(message);
                throw new DataException(message);
            }

            return members;
        }
    }
}