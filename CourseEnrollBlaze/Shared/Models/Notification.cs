using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseEnrollBlaze.Shared.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }  
        public string RecipientId { get; set; } = string.Empty;

        public string? Link { get; set; } 
        public string TimeSinceStr
        {
            get
            {
                var difference = DateTime.Now - CreatedAt;

                if (difference.Days > 0)
                    return difference.Days.ToString() + " day(s) ago";

                if (difference.Hours > 0)
                    return difference.Hours.ToString() + " hour(s) ago";

                return difference.Minutes.ToString() + " min(s) ago";
            }
        }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
