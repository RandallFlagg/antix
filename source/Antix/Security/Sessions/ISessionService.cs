﻿namespace Antix.Security.Sessions
{
    public interface ISessionService
    {
        Session Login(string identifier, string password);
        void Logout(string identifier);
        Session Authenticate(string identifier);
    }
}