
namespace CourseEnrollBlaze.Shared.Entities
{
    public class Enrollment
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public UserAccount? User { get; set; } 

        public Guid CourseId { get; set; }
        public Course? Course { get; set; } 

        public Enrollment()
        {
            Id = Guid.NewGuid();
        }
    }

}
