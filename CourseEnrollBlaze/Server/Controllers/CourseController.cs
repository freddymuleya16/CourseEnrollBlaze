using CourseEnrollBlaze.Server.Interfaces;
using CourseEnrollBlaze.Server.Services;
using CourseEnrollBlaze.Shared.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseEnrollBlaze.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CourseController> _logger;

        public CourseController(ICourseService courseService, ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shared.Entities.Course>>> GetCourses()
        {
            try
            {
                var courses = await _courseService.GetCoursesAsync();
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving courses.");
                return StatusCode(500, "An error occurred while retrieving courses. Please try again later.");
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Shared.Entities.Course>> GetCourseById(Guid id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                if (course == null)
                {
                    return NotFound();
                }
                return Ok(course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving course by ID.");
                return StatusCode(500, "An error occurred while retrieving course by ID. Please try again later.");
            }
        }


        [HttpPost]
        public async Task<ActionResult<Shared.Entities.Course>> CreateCourse(Shared.Entities.Course course)
        {
            try
            {
                var createdCourse = await _courseService.CreateCourseAsync(course);
                return CreatedAtAction(nameof(GetCourseById), new { id = createdCourse.Id }, createdCourse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating course.");
                return StatusCode(500, "An error occurred while creating course. Please try again later.");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(Guid id, Shared.Entities.Course course)
        {
            try
            {
                if (id != course.Id)
                {
                    return BadRequest("Course ID in the request body does not match the ID in the URL.");
                }

                var updatedCourse = await _courseService.UpdateCourseAsync(course);
                if (updatedCourse == null)
                {
                    return NotFound("Course not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating course.");
                return StatusCode(500, "An error occurred while updating course. Please try again later.");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                if (course == null)
                {
                    return NotFound("Course not found.");
                }

                var deleted = await _courseService.DeleteCourseAsync(id);
                if (!deleted)
                {
                    return StatusCode(500, "An error occurred while deleting course. Please try again later.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting course.");
                return StatusCode(500, "An error occurred while deleting course. Please try again later.");
            }
        }


        [HttpGet("enrolled")]
        public async Task<IActionResult> GetCoursesStudentIsEnrolledAsync(Guid userId)
        {
            try
            {
                var courses = await _courseService.GetCoursesStudentIsEnrolledAsync(userId);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving enrolled courses for user.");
                return StatusCode(500, "An error occurred while retrieving enrolled courses. Please try again later.");
            }
        }


        [HttpGet("notenrolled")]
        public async Task<IActionResult> GetCoursesStudentIsNotEnrolledAsync(Guid userId)
        {
            try
            {
                var courses = await _courseService.GetCoursesStudentIsNotEnrolledAsync(userId);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving courses not enrolled by the user.");
                return StatusCode(500, "An error occurred while retrieving courses not enrolled by the user. Please try again later.");
            }
        }


        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollCourse([FromBody] EnrollCourseRequest request)
        {
            try
            {
                if (await _courseService.EnrollCourseAsync(request.CourseId, request.UserId))
                {
                    return Ok();  
                }
                else
                {
                    return BadRequest("Course enrollment failed.");  
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while enrolling user in course.");
                return StatusCode(500, "An error occurred while enrolling user in course. Please try again later.");
            }
        }


        [HttpPost("deregister")]
        public async Task<IActionResult> DeregisterCourse([FromBody] EnrollCourseRequest request)
        {
            try
            {
                if (await _courseService.DeregisterCourseAsync(request.CourseId, request.UserId))
                {
                    return Ok();  
                }
                else
                {
                    return BadRequest("Course deregistration failed.");  
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deregistering user from course.");
                return StatusCode(500, "An error occurred while deregistering user from course. Please try again later.");
            }
        }


        [HttpGet("instructor/{instructorId}")]
        public async Task<IActionResult> GetInstructorCourses(Guid instructorId)
        {
            try
            {
                var courses = await _courseService.GetInstructorCoursesAsync(instructorId);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving instructor courses.");
                return StatusCode(500, "An error occurred while retrieving instructor courses. Please try again later.");
            }
        }


    }
}
