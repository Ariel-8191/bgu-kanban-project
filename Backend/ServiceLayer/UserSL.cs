namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Class representing a user in the service layer.
    /// </summary>
    public class UserSL
    {
        public string Email { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSL"/> class.
        /// </summary>
        /// <param name="userBL"></param>
        internal UserSL(UserBL userBL)
        {
            this.Email = userBL.Email;
        }
    }
}
