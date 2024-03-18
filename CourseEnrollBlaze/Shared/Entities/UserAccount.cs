using System.ComponentModel.DataAnnotations;

namespace CourseEnrollBlaze.Shared.Entities
{
    public class UserAccount
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name must be no more than 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name must be no more than 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Student number is required")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Student number must be 6 digits")]
        public string StudentNumber { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsEmailConfirmed { get; set; } = false;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public UserAccount()
        {
            Id = Guid.NewGuid();
        }
    }
}
