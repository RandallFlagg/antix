﻿using System;

namespace Example.MvcApplication.Sessions
{
    public class Session
    {
        public string Identifier { get; set; }
        public SessionUser User { get; set; }

        public DateTime LoginOn { get; set; }
        public DateTime? LogoutOn { get; set; }

        public DateTime ExpiresOn { get; set; }
    }
}