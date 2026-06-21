using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// Controller responsible for executing database operations for user data.
    /// </summary>
    internal class UserController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string connectionString;
        private const string userTableName = "Users";
        private const string emailColumnName = "email";
        private const int emailColumnIndex = 0;
        private const string passwordColumnName = "password";
        private const int passwordColumnIndex = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class and configures the database connection string.
        /// </summary>
        public UserController()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            this.connectionString = $"Data Source={path};Version=3;Foreign Keys=True;";
        }

        /// <summary>
        /// Inserts a new user record into the database.
        /// </summary>
        /// <param name="user">The user data access object containing the details to insert.</param>
        public void Insert(UserDAL user)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"INSERT INTO {userTableName} ({emailColumnName}, {passwordColumnName}) VALUES (@Email, @Password)";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@Email", user.Email);
                        command.Parameters.AddWithValue("@Password", user.Password);
                        command.ExecuteNonQuery();
                    }
                }
                log.Info($"Successfully inserted user '{user.Email}' into the database.");
            }
            catch (SQLiteException)
            {
                string message = $"Failed to insert user '{user.Email}' into the database.";
                log.Error(message);
                throw new DataException(message);
            }
        }

        /// <summary>
        /// Retrieves all registered users from the database.
        /// </summary>
        /// <returns>A list of <see cref="UserDAL"/> objects representing all users.</returns>
        /// <exception cref="SQLiteException">Thrown when a database error occurs during retrieval.</exception>
        public List<UserDAL> SelectAll()
        {
            List<UserDAL> users = new List<UserDAL>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"SELECT {emailColumnName}, {passwordColumnName} FROM {userTableName}";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(ConvertReaderToUser(reader));
                            }
                        }
                    }
                }
                log.Debug($"Successfully retrieved {users.Count} users from the database.");
            }
            catch (SQLiteException)
            {
                string message = "An error occurred while retrieving all users from the database.";
                log.Error(message);
                throw new DataException(message);
            }
            return users;
        }

        /// <summary>
        /// Deletes all user records from the database.
        /// </summary>
        /// <exception cref="SQLiteException">Thrown when a database error occurs during deletion.</exception>
        public void DeleteAll()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string commandText = $"DELETE FROM {userTableName}";
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        log.Info($"Successfully deleted all users from the database. Rows affected: {rowsAffected}");
                    }
                }
            }
            catch (SQLiteException)
            {
                string message = "An error occurred while attempting to delete all users from the database.";
                log.Error(message);
                throw new DataException(message);
            }
        }

        /// <summary>
        /// Converts the current row of a SQLite data reader into a <see cref="UserDAL"/> object.
        /// </summary>
        /// <param name="reader">The active SQLite data reader.</param>
        /// <returns>A populated <see cref="UserDAL"/> instance.</returns>
        private UserDAL ConvertReaderToUser(SQLiteDataReader reader)
        {
            string email = reader.GetString(emailColumnIndex);
            string password = reader.GetString(passwordColumnIndex);
            return new UserDAL(email, password, true);
        }
    }
}
