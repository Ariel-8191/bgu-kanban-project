namespace Frontend.Model
{
    /// <summary>
    /// A frontend model representing the currently logged-in user.
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// The email of the user, which also serves as their unique identifier.
        /// </summary>
        public string Email { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserModel"/> class.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        public UserModel(string email)
        {
            Email = email;
        }
    }
}
