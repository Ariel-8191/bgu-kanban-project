using IntroSE.Kanban.Backend.BusinessLayer.CrossCutting;
using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.BusinessLayer.User
{
    internal class UserFacade
    {
        private Dictionary<string, UserBL> users;
        private AuthenticationFacade authenticationFacade;

        public UserFacade()
        {
            throw new NotImplementedException();
        }

        public UserBL Register(string email, string password)
        {
            throw new NotImplementedException();
        }

        public UserBL Login(string email, string password)
        {
            throw new NotImplementedException();
        }

        public void Logout(string email)
        {
            throw new NotImplementedException();
        }
    }
}
