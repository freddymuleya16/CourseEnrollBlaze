using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseEnrollBlaze.Shared.RequestModels
{
    public class EnrollCourseRequest
    {
        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }
    }
}
