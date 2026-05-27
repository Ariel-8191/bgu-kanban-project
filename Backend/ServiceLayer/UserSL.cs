namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Represents a user in the service layer.
    /// </summary>
    public class UserSL
    {
        public string Email { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSL"/> class.
        /// </summary>
        /// <param name="userBL">The user instance from the businnes layer that the new instance represents</param>
        internal UserSL(UserBL userBL)
        {
            this.Email = userBL.Email;
        }
    }
}
