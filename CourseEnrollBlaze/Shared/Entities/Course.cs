using System.ComponentModel.DataAnnotations;

namespace CourseEnrollBlaze.Shared.Entities
{
    public class Course
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Course name is required")]
        [StringLength(100, ErrorMessage = "Course name must be between 3 and 100 characters", MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description must be between 10 and 500 characters", MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;

        [Range(1, 1000, ErrorMessage = "Maximum students must be between 1 and 1000")]
        public int MaxStudents { get; set; }

        [Range(typeof(DateTime), "1/1/2022", "1/1/2025", ErrorMessage = "Start date must be between 1/1/2022 and 1/1/2025")]
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(5);

        [Range(typeof(DateTime), "1/1/2022", "1/1/2025", ErrorMessage = "End date must be between 1/1/2022 and 1/1/2025")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(10);


        public bool IsValid => StartDate < EndDate;

        public string IsValidErrorMessage => "End date must be later than start date";
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public Guid InstructorId { get; set; }

    }

}
