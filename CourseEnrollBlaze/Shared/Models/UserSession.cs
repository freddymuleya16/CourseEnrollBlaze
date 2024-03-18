﻿namespace CourseEnrollBlaze.Shared.Models
{
    public class UserSession
    {
        public string Id { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public int ExpiresIn { get; set; }
        public DateTime ExpiresInTimeStamp { get; set; }


    }
}
