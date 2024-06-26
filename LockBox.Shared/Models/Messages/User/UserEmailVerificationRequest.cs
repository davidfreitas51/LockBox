﻿using System.ComponentModel.DataAnnotations;

namespace LockBox.Commons.Models.Messages.User
{
    public class UserEmailVerificationRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
