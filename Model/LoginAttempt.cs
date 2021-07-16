
using System;
using System.Collections.Generic;

namespace MSN.Model
{
    public class LoginAttempt
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ReturnUri { get; set; } = string.Empty;
        public bool IsProcessed { get; set; } = false;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public DateTime DateCreated { get; set; } = DateTime.Now;

    }
}