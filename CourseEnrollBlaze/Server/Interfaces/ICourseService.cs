using CourseEnrollBlaze.Shared.Entities;

namespace CourseEnrollBlaze.Server.Interfaces
{
    public interface ICourseService
    {
        Task<bool> DeleteCourseAsync(Guid courseId);
        Task<Course> UpdateCourseAsync(Course course);
        Task<Course> CreateCourseAsync(Course course);
        Task<List<Course>> GetCoursesAsync();
        Task<Course> GetCourseByIdAsync(Guid courseId); 
        Task<List<Course>> GetCoursesStudentIsEnrolledAsync(Guid userId);
        Task<List<Course>> GetCoursesStudentIsNotEnrolledAsync(Guid userId);
        Task<bool> EnrollCourseAsync(Guid courseId, Guid userId);
        Task<bool> DeregisterCourseAsync(Guid courseId, Guid userId);
        Task<IEnumerable<Course>> GetInstructorCoursesAsync(Guid instructorId);
    }
}
