using CourseEnrollBlaze.Server.Context;
using CourseEnrollBlaze.Server.Interfaces;
using CourseEnrollBlaze.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollBlaze.Server.Services
{
    public class CourseService : ICourseService
    {
        private readonly CEBDbContext _dbContext;
        private readonly INotificationService _notificationService;

        public CourseService(CEBDbContext dbContext, INotificationService notificationService)
        {
            _dbContext = dbContext;
            _notificationService = notificationService;
        }

        public async Task<bool> DeleteCourseAsync(Guid courseId)
        {
            var course = await _dbContext.Courses.FindAsync(courseId);
            if (course == null)
            {
                return false;
            }

            var enrolledUsers = await _dbContext.Enrollments
                .Where(e => e.CourseId == courseId)
                .ToListAsync();

            foreach (var enrollment in enrolledUsers)
            {
                await _notificationService.AddNotificationAsync("Course Deleted", $"The course '{course.Name}' has been deleted.", enrollment.UserId.ToString());

                _dbContext.Enrollments.Remove(enrollment);
            }

            _dbContext.Courses.Remove(course);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<Course?> UpdateCourseAsync(Course course)
        {
            _dbContext.Entry(course).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(course.Id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

            var enrolledUsers = await _dbContext.Enrollments
                .Where(e => e.CourseId == course.Id)
                .Select(e => e.UserId)
                .ToListAsync();

            foreach (var userId in enrolledUsers)
            {
                await _notificationService.AddNotificationAsync("Course Update", $"The course '{course.Name}' has been updated.", userId.ToString());
            }

            return course;
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            _dbContext.Courses.Add(course);
            await _dbContext.SaveChangesAsync();

            var studentUsers = await _dbContext.UserAccounts.Where(u => u.Role == "Student").ToListAsync();

            foreach (var student in studentUsers)
            {
                await _notificationService.AddNotificationAsync("New Course", $"A new course '{course.Name}' has been created.", student.Id.ToString());
            }
            return course;
        }

        private bool CourseExists(Guid courseId)
        {
            return _dbContext.Courses.Any(e => e.Id == courseId);
        }

        public async Task<List<Course>> GetCoursesAsync()
        {
            return await _dbContext.Courses.ToListAsync();
        }

        public async Task<Course> GetCourseByIdAsync(Guid courseId)
        {
            return await _dbContext.Courses.FindAsync(courseId);
        }

        public async Task SeedCoursesAsync()
        {
            if (!_dbContext.Courses.Any())
            {
                _dbContext.Courses.AddRange(new List<Course>
            {
                new Course { Name = "Introduction to Blazor", Description = "Learn the basics of Blazor framework", MaxStudents = 50, StartDate = DateTime.Parse("2022-01-01"), EndDate = DateTime.Parse("2022-02-01") },
                new Course { Name = "Advanced Blazor Techniques", Description = "Explore advanced features of Blazor", MaxStudents = 30, StartDate = DateTime.Parse("2022-03-01"), EndDate = DateTime.Parse("2022-04-01") },

            });
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Course>> GetCoursesStudentIsEnrolledAsync(Guid userId)
        {
            var res = await _dbContext.Courses
            .Where(c => c.Enrollments.Any(e => e.UserId == userId))
            .ToListAsync();
            return res;
        }

        public async Task<List<Course>> GetCoursesStudentIsNotEnrolledAsync(Guid userId)
        {
            var res = await _dbContext.Courses
            .Where(c => !c.Enrollments.Any(e => e.UserId == userId))
            .ToListAsync();
            return res;
        }

        public async Task<bool> EnrollCourseAsync(Guid courseId, Guid userId)
        {
            var course = await _dbContext.Courses.FindAsync(courseId);
            if (course != null && course.MaxStudents > 0)
            {
                course.MaxStudents--;

                _dbContext.Enrollments.Add(new Enrollment { CourseId = courseId, UserId = userId });

                await _dbContext.SaveChangesAsync();
                 
                var instructor = await _dbContext.UserAccounts.FindAsync(course.InstructorId);
                if (instructor != null)
                {
                    var student = await _dbContext.UserAccounts.FindAsync(userId);
                    var studentName = $"{student?.FirstName} {student?.LastName}";

                    await _notificationService.AddNotificationAsync("New Enrollment", $"Student {studentName} has enrolled in your course: {course.Name}", instructor.Id.ToString());
                }

                return true;
            }

            return false;
        }

        public async Task<bool> DeregisterCourseAsync(Guid courseId, Guid userId)
        {
            var course = await _dbContext.Courses.FindAsync(courseId);
            var enrolment = await _dbContext.Enrollments.FirstOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId);
            if (course != null && enrolment != null)
            {
                course.MaxStudents++;

                _dbContext.Enrollments.Remove(enrolment);

                await _dbContext.SaveChangesAsync();

                var instructor = await _dbContext.UserAccounts.FindAsync(course.InstructorId);
                if (instructor != null)
                {
                    var student = await _dbContext.UserAccounts.FindAsync(userId);
                    var studentName = $"{student?.FirstName} {student?.LastName}";
                    await _notificationService.AddNotificationAsync("Deregistration", $"Student {studentName} has deregistered from your course: {course.Name}", instructor.Id.ToString());
                }

                return true;
            }

            return false;
        }

        public async Task<IEnumerable<Course>> GetInstructorCoursesAsync(Guid instructorId)
        {
            return await _dbContext.Courses
                .Where(c => c.InstructorId == instructorId)
                .ToListAsync();
        }
    }
}