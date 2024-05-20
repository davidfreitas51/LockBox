﻿using LockBox.Models;

namespace LockBox.Commons.Models.Messages.RegisteredAccount
{
    public class RAGetByUserRequest
    {
        public string Token { get; set; }
        public AppUser AppUser { get; set; }
    }
}
