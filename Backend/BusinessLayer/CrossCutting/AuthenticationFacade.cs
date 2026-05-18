using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.BusinessLayer.CrossCutting
{
    internal class AuthenticationFacade
    {
        private HashSet<string> loggedInUsers;

        public AuthenticationFacade()
        {
            throw new NotImplementedException();
        }

        public bool IsLoggedIn(string email)
        {
            throw new NotImplementedException();
        }

        public void Login()
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }
    }
}
