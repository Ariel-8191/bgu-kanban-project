using System;

namespace IntroSE.Kanban.Backend.BusinessLayer.User
{
    internal class UserBL
    {
        public string Email { get; }
        private string _password;

        private string Password
        {
            get { return _password; }
            set
            {
                throw new NotImplementedException();
            }
        }
       
        public UserBL(string email, string password)
        {
            throw new NotImplementedException();
        }

        public bool CheckPassword(string password)
        {
            throw new NotImplementedException();
        }



    }
}
