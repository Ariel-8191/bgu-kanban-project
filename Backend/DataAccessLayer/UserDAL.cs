namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// Represents a user within the data access layer of the Kanban application.
    /// </summary>
    internal class UserDAL
    {
        private UserController userController;
        private bool isPersisted;

        public string Email { get; }
        internal string Password { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="UserDAL"/> class.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="password">The password for the user.</param>
        public UserDAL(string email, string password)
        {
            this.userController = new UserController();
            this.isPersisted = false;

            this.Email = email;
            this.Password = password;
        }

        /// <summary>
        /// Persists the current instance by inserting it into the database if it hasn't been saved yet.
        /// </summary>
        public void Persist()
        {
            if (!isPersisted)
            {
                userController.Insert(this);
                isPersisted = true;
            }
        }
    }
}
